using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public LayerMask groundLayer; // Set this in the Inspector to the layer of the ground or object you want to check against
    private Vector3 startPosition,startRotation;
    [Range(-1f,1f)]
    public float accelaration,rotation;

    public float timeSinceStart = 0f;

    [Header("Fitness")]
    public float overallFitness;

    public float distanceWeight = 1.4f;
    public float avgSpeedWeight = 0.2f;

    public float avgSensorWeight = 0.1f;

    [Header("Network Options")]
    public int LAYERS = 1;
    public int NEURONS = 10;

    // [Header("Center Camera")]
    // public Camera centerCamera;



    private CameraImageProcessor cameraImageProcessor;
    private Vector3 lastPosition;
    private float totalDistanceTravelled;

    private float avgSpeed;

    private float aSensor,bSensor,cSensor;

    private NeuralNetwork network;

    // private int checkCenterCamera = 0;


    private void Awake(){
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        network = GetComponent<NeuralNetwork>();
        // cameraImageProcessor = new CameraImageProcessor();
    }

    public void Reset(){
        timeSinceStart=0f;
        totalDistanceTravelled=0f;
        avgSpeed=0f;
        lastPosition=startPosition;
        overallFitness=0f;
        transform.position = startPosition;
        transform.eulerAngles= startRotation;
    }

    public void ResetWithNetwork (NeuralNetwork net){
            network = net;
            Reset();
    }
    private void OnCollisionEnter(Collision collision){
        Death();
    }

    private Vector3 pos;
    public void MoveCar(float acc, float rotation){
        pos = Vector3.Lerp(Vector3.zero,new Vector3(0,0,acc*11f),0.02f);
        pos = transform.TransformDirection(pos);
        transform.position+=pos;

        transform.eulerAngles += new Vector3(0,(rotation*90)*0.02f,0);

    }

    private void InputSensors(){
        Vector3 a = (transform.forward+transform.right); // front right
        Vector3 b = transform.forward; // front 
        Vector3 c = (transform.forward-transform.right); // front left

        Ray rayTowardsA = new Ray(transform.position,a);
        Ray rayTowardsB = new Ray(transform.position,b);
        Ray rayTowardsC = new Ray(transform.position,c);

        RaycastHit hit;
         
        if (Physics.Raycast(rayTowardsA,out hit)){
            aSensor = hit.distance/30;
            Debug.DrawLine(rayTowardsA.origin, hit.point, Color.red);
        }
        if (Physics.Raycast(rayTowardsB,out hit)){
            bSensor = hit.distance/30;
            Debug.DrawLine(rayTowardsB.origin, hit.point, Color.blue);
        }
        if (Physics.Raycast(rayTowardsC,out hit)){
            cSensor = hit.distance/30;
            Debug.DrawLine(rayTowardsC.origin, hit.point, Color.yellow);
        }
    }

    private void FixedUpdate(){
        InputSensors();
        
        lastPosition = transform.position;

        // temp name
        List<float> position = new List<float>();
        position.Add(aSensor);
        position.Add(bSensor);
        position.Add(cSensor);
        (accelaration, rotation) = network.RunNetwork(position);

        MoveCar(accelaration, rotation);

        timeSinceStart+= Time.deltaTime;

        CalculateFitness();

        // checkCenterCamera+=1;
        // if(checkCenterCamera%100==0)
        //     CaptureSnapshot();

    }

    // void CaptureSnapshot(){
    //     // Create a RenderTexture with the same dimensions as the screen
    //     RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
    //     Camera mainCamera = centerCamera;

    //     // Set the target texture of the main camera to the RenderTexture
    //     mainCamera.targetTexture = renderTexture;

    //     // Render the camera's view to the RenderTexture
    //     mainCamera.Render();

    //     // Read the pixels from the RenderTexture
    //     RenderTexture.active = renderTexture;
    //     Texture2D snapshot = new Texture2D(Screen.width, Screen.height);
    //     snapshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
    //     snapshot.Apply();
    //     RenderTexture.active = null;

    //     // Reset the target texture of the main camera
    //     mainCamera.targetTexture = null;

    //     // Encode the Texture2D as a PNG file
    //     byte[] bytes = snapshot.EncodeToPNG();

    //     // Save the PNG file to disk (change the file path as needed)
    //     string filePath = Application.dataPath + "/Snapshot.png";
    //     System.IO.File.WriteAllBytes(filePath, bytes);
    // }


    private bool isAboveTrack(){
        // Cast a ray from the car's position towards the downward direction
        Ray ray = new Ray(transform.position, Vector3.down);

        // Set the maximum distance the ray can travel
        float maxRayDistance = 100.0f; // Adjust this distance based on your scene

        // Perform the raycast
        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            // The ray hit something
            Debug.Log("Ray hit an object.");

            // Access information about the hit object
            GameObject objectBelowCar = hit.collider.gameObject;

            if(objectBelowCar.name== "Road")
                return true;
            // Do something with the objectBelowCar, for example:
            Debug.Log("Object below car: " + objectBelowCar.name);
        }
        else
        {
            // The ray did not hit anything
            Debug.Log("Ray did not hit any object.");
        }
        return false;
    }

    private void CalculateFitness(){
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled/timeSinceStart;
        float avgSensor = (aSensor+bSensor+cSensor)/3;
        overallFitness =  totalDistanceTravelled*distanceWeight + avgSpeed*avgSpeedWeight + avgSensor*avgSensorWeight;

        if(timeSinceStart > 20 && overallFitness < 50){
            Death();
        }
        isAboveTrack();
        if(overallFitness>1000){
            // Saves the network to a json
            Death();
        }
    }

    private void Death(){
         GameObject.FindObjectOfType<GeneticAlgorithmManager>().Death(overallFitness);
    }

}
