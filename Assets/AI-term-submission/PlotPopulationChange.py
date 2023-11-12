import json
import matplotlib.pyplot as plt

# List of JSON data
data_list = [
    {
        "numberOfGenerations": 10,
        "mutationRate": 0.15000000596046449,
        "bestAgentSelection": 2,
        "worstAgentSelection": 0,
        "numberToCrossover": 5,
        "populationSize": 10,
        "fitnessAccrossGen": [10.875022888183594, 14.421631813049317, 173.17576599121095, 254.02511596679688, 253.5361785888672, 253.5361785888672, 254.87692260742188, 255.83660888671876, 255.83660888671876, 255.83660888671876]
    },
    {
        "numberOfGenerations": 10,
        "mutationRate": 0.15000000596046449,
        "bestAgentSelection": 4,
        "worstAgentSelection": 1,
        "numberToCrossover": 10,
        "populationSize": 20,
        "fitnessAccrossGen": [15.624343872070313, 43.61760711669922, 43.608848571777347, 43.61935806274414, 74.4548568725586, 251.4388885498047, 253.570068359375, 253.36192321777345, 253.36325073242188, 253.36184692382813]
    },
    {
        "numberOfGenerations": 10,
        "mutationRate": 0.15000000596046449,
        "bestAgentSelection": 6,
        "worstAgentSelection": 2,
        "numberToCrossover": 15,
        "populationSize": 30,
        "fitnessAccrossGen": [12.341798782348633, 76.03890228271485, 250.2239227294922, 250.545166015625, 250.55514526367188, 251.5832977294922, 256.12158203125, 256.1240234375, 256.125, 257.48681640625]
    }
]

# Plotting
plt.figure(figsize=(10, 6))
for data in data_list:
    fitness_data = data["fitnessAccrossGen"]
    plt.plot(range(1, len(fitness_data) + 1), fitness_data, marker='o', label=f'Population Size: {data["populationSize"]}')

plt.xlabel('Generation')
plt.ylabel('Fitness')
plt.title('Effect of Population Size on Fitness Across Generations')
plt.legend()
plt.grid(True)
plt.show()
