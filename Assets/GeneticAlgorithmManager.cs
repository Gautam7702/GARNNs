using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using System.Linq;
using static Unity.VisualScripting.LudiqRootObjectEditor;

public class GeneticAlgorithmManager : MonoBehaviour
{
    [Header("References")]
    public CarController controller;

    [Header("Controls")]
    public int initialPopulation = 85;

    [Range(0.0f, 1.0f)]
    public float mutationRate = 0.055f;

    [Header("Crossover Controls")]
    public int bestAgentSelection = 8;
    public int worstAgentSelection = 3;
    public int numberToCrossover = 39;

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome = 0;


    private int naturallySelected;
    private List<int> genePool = new();
    private NeuralNetwork[] population;
    //private ExperimentData experimentData = new(10);

    private void Start()
    {
        GeneratePopulation<NeuralNetwork>();
    }

    private void GeneratePopulation<T>() where T : NeuralNetwork, new()
    {
        population = new T[initialPopulation];
        Populate(population, 0);
        ResetToCurrentGenome();

        //experimentData.numberOfGenerations = 10;
        //experimentData.bestAgentSelection = bestAgentSelection;
        //experimentData.worstAgentSelection = worstAgentSelection;
        //experimentData.numberToCrossover = numberToCrossover;
        //experimentData.mutationRate = mutationRate;
        //experimentData.populationSize = initialPopulation;
    }

    private void Populate<T>(T[] newPopulation, int startingIndex) where T : NeuralNetwork, new()
    {
        while (startingIndex < initialPopulation)
        {
            newPopulation[startingIndex] = new T();
            newPopulation[startingIndex++].Initialize(3, 2, controller.LAYERS, controller.NEURONS);
        }
    }

    private void ResetToCurrentGenome() => controller.ResetWithNetwork(population[currentGenome]);

    public void Death(float fitness)
    {
        if (currentGenome < population.Length - 1)
        {
            population[currentGenome++].fitness = fitness;
            ResetToCurrentGenome();
        }
        else
        {
            RePopulate<NeuralNetwork>();
        }
    }

    private void RePopulate<T>() where T : NeuralNetwork, new()
    {
        genePool.Clear();
        if (currentGeneration == 15)
        {
            //string jsonString = JsonUtility.ToJson(experimentData);
            //string fileName = $"/{initialPopulation}-{mutationRate}-{bestAgentSelection}-{worstAgentSelection}-{numberToCrossover}-GA.json";
            //string filePath = Application.persistentDataPath + fileName;

            //File.WriteAllText(filePath, jsonString);
            //Time.timeScale = 0;
            return;
        }

        naturallySelected = 0;
        SortPopulation();

        //experimentData.fitnessAccrossGen[currentGeneration++] = population[0].fitness;
        currentGeneration++;
        T[] newPopulation = PickBestPopulation<T>();
        Crossover(newPopulation);
        Mutate(newPopulation);
        Populate(newPopulation, startingIndex: naturallySelected);

        population = newPopulation;
        currentGenome = 0;
        ResetToCurrentGenome();
    }

    private T[] PickBestPopulation<T>() where T : NeuralNetwork, new()
    {
        T[] newPopulation = new T[initialPopulation];

        for (int i = 0; i < bestAgentSelection; i++)
        {
            newPopulation[naturallySelected] = population[i] as T;
            newPopulation[naturallySelected++].fitness = 0;

            for (int j = 0; j <= Mathf.RoundToInt(population[i].fitness * 10); j++)
            {
                genePool.Add(i);
            }
        }

        for (int i = 0; i < worstAgentSelection; i++)
        {
            int last = population.Length - 1 - i;

            for (int j = 0; j <= Mathf.RoundToInt(population[last].fitness * 10); j++)
            {
                genePool.Add(last);
            }
        }

        return newPopulation;
    }

    private void Crossover<T>(T[] newPopulation) where T : NeuralNetwork, new() 
    {
        for (int i = 0; i < numberToCrossover; i++)
        {
            int individual1 = genePool[Random.Range(0, genePool.Count)], individual2 = genePool[Random.Range(0, genePool.Count)];
 
            for (int j = 0; j < 100; j++)
            {
                if (individual1 != individual2)
                    break;

                individual1 = genePool[Random.Range(0, genePool.Count)];
                individual2 = genePool[Random.Range(0, genePool.Count)];
            }

            var (child1, child2) = typeof(T) == typeof(RecurrentNeuralNetwork)
                            ? RecurrentNeuralNetwork.Crossover(newPopulation[individual1], newPopulation[individual2])
                            : NeuralNetwork.Crossover(newPopulation[individual1], newPopulation[individual2]);

            if (naturallySelected >= newPopulation.Length)
                return;

            newPopulation[naturallySelected++] = child1 as T;
            newPopulation[naturallySelected++] = child2 as T;
        }
    }

    private void Mutate<T>(T[] newPopulation) where T : NeuralNetwork
    {
        for (int i = 0; i < naturallySelected; i++)
        {
            newPopulation[i].Mutate(mutationRate);
        }
    }

    private void SortPopulation()
    {
        for (int i = 0; i < population.Length; i++)
        {
            for (int j = i; j < population.Length; j++)
            {
                if (population[i].fitness < population[j].fitness)
                {
                    (population[j], population[i]) = (population[i], population[j]);
                }
            }
        }
    }

}
