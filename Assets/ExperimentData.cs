using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ExperimentData
{
    public ExperimentData(int numberOfGenerations){
        fitnessAccrossGen = new float[numberOfGenerations];
    }
    public int numberOfGenerations;
    public float mutationRate;
    public int bestAgentSelection;
    public int worstAgentSelection;
    public int numberToCrossover;

    public int populationSize;

    public float[] fitnessAccrossGen;

}
