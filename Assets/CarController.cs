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
    public int numberOfSensors = 5;

    // [Header("Center Camera")]
    // public Camera centerCamera;

    List<float> sensors =  new List<float>();

    //private CameraImageProcessor cameraImageProcessor;
    private Vector3 lastPosition;
    private float totalDistanceTravelled;

    private float avgSpeed;

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
        if(sensors.Count==0){
            for (int i = 0; i < numberOfSensors; i++){
                sensors.Add(i + 1); // Replace this with your actual sensor values
            }
        }
        for (int i = 0; i < numberOfSensors; i++){
            float angle = Mathf.PI / 6.0f + ((float)i/(float)(numberOfSensors-1))*2*Mathf.PI / 3.0f;

            Vector3 direction = new Vector3(Mathf.Cos(angle), 0.0f, Mathf.Sin(angle));
            Ray ray = new Ray(transform.position, transform.TransformDirection(direction));

            RaycastHit hit;
            System.Random random = new System.Random();
            float noise = (float)(random.NextDouble());
            if (Physics.Raycast(ray, out hit))
            {
                sensors[i] = (hit.distance)/ 30.0f;
                // Debug.DrawLine(ray.origin, hit.point);
            }else{
                sensors[i] = 0;
            }
            sensors[i]+= 0.3f*noise;
        }
    }

    private void FixedUpdate(){
        InputSensors();
        
        lastPosition = transform.position;

        // temp name
        (accelaration, rotation) = network.RunNetwork(sensors);

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
        Ray ray = new Ray(transform.position, Vector3.down);
        float maxRayDistance = 100.0f;

        if (Physics.Raycast(ray, out RaycastHit hit, maxRayDistance))
        {
            GameObject objectBelowCar = hit.collider.gameObject;

            if(objectBelowCar.name== "Road")
                return true;
        }

        return false;
    }

    private void CalculateFitness(){
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled/timeSinceStart;
        float avgSensor = 0;
        overallFitness =  totalDistanceTravelled*distanceWeight + avgSpeed*avgSpeedWeight + avgSensor*avgSensorWeight;

        if(timeSinceStart > 20 && overallFitness < 50){
            Death();
        }
        isAboveTrack();
        if(overallFitness>1000){
            Death();
        }
    }

    private void Death(){
         GameObject.FindObjectOfType<GeneticAlgorithmManager>().Death(overallFitness);
    }

}
