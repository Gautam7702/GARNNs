using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

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



    private Vector3 lastPosition;
    private float totalDistanceTravelled;

    private float avgSpeed;

    private float aSensor,bSensor,cSensor;

    private NeuralNetwork network;


    private void Awake(){
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
        network = GetComponent<NeuralNetwork>();
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

        (accelaration, rotation) = network.RunNetwork(aSensor, bSensor, cSensor);


        MoveCar(accelaration, rotation);

        timeSinceStart+= Time.deltaTime;

        CalculateFitness();

    }


    private void CalculateFitness(){
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled/timeSinceStart;
        float avgSensor = (aSensor+bSensor+cSensor)/3;
        overallFitness =  totalDistanceTravelled*distanceWeight + avgSpeed*avgSpeedWeight + avgSensor*avgSensorWeight;

        if(timeSinceStart > 20 && overallFitness < 40){
            Death();
        }

        if(overallFitness> 1000){
            // Saves the network to a json

            Death();
        }
    }

    private void Death(){
         GameObject.FindObjectOfType<GeneticAlgorithmManager>().Death(overallFitness);
    }

}
