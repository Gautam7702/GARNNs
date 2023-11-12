import json
import matplotlib.pyplot as plt

# List of JSON data
data_list = [
    {
        "numberOfGenerations": 10,
        "mutationRate": 0.15000000596046449,
        "bestAgentSelection": 4,
        "worstAgentSelection": 2,
        "numberToCrossover": 0,
        "populationSize": 20,
        "fitnessAccrossGen": [17.939435958862306, 237.02462768554688, 238.90049743652345, 36.556617736816409, 36.55661392211914, 30.73499298095703, 30.38064956665039, 201.947265625, 201.94064331054688, 210.85910034179688]
    },
    {
        "numberOfGenerations": 10,
        "mutationRate": 0.15000000596046449,
        "bestAgentSelection": 4,
        "worstAgentSelection": 2,
        "numberToCrossover": 7,
        "populationSize": 20,
        "fitnessAccrossGen": [74.27288818359375, 74.27088165283203, 104.66133880615235, 104.65499114990235, 199.642578125, 202.85281372070313, 242.51589965820313, 225.24330139160157, 225.2432098388672, 225.47531127929688]
    },
    {
        "numberOfGenerations": 10,
        "mutationRate": 0.15000000596046449,
        "bestAgentSelection": 4,
        "worstAgentSelection": 2,
        "numberToCrossover": 14,
        "populationSize": 20,
        "fitnessAccrossGen": [23.701030731201173, 23.70109748840332, 23.477581024169923, 69.09672546386719, 200.74696350097657, 205.28656005859376, 205.6641387939453, 1000.035888671875, 1000.0989990234375, 1000.1329345703125]
    }
]

# Plotting
plt.figure(figsize=(10, 6))
for data in data_list:
    fitness_data = data["fitnessAccrossGen"]
    plt.plot(range(1, len(fitness_data) + 1), fitness_data, marker='o', label=f'Number To Crossover: {data["numberToCrossover"]}')

plt.xlabel('Generation')
plt.ylabel('Fitness')
plt.title('Effect of Number To Crossover on Fitness Across Generations')
plt.legend()
plt.grid(True)
plt.show()
