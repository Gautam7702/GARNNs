import json
import matplotlib.pyplot as plt

# List of file names
file_names = ["0-8-3-25-GA.json", "0.05-8-3-25-GA.json", "0.15-8-3-25-GA.json", "0.25-8-3-25-GA.json","0.5-8-3-25-GA.json"]

# Initialize empty lists to store mutation rates and fitness values
mutation_rates = []
fitness_values = []

# Iterate through the files
for file_name in file_names:
    file_path = "./Output_ANN/"+ file_name
    with open(file_path, 'r') as file:
        data = json.load(file)
        mutation_rate = data["mutationRate"]
        fitness_in_last_gen = data["fitnessAccrossGen"][-1]
        mutation_rates.append(mutation_rate)
        fitness_values.append(fitness_in_last_gen)

# Plotting
plt.figure(figsize=(8, 6))
plt.plot(mutation_rates, fitness_values, marker='o', linestyle='-')
plt.xlabel('Mutation Rate')
plt.ylabel('Fitness in Last Generation')
plt.title('Effect of Mutation Rate on Fitness')
plt.grid(True)
plt.show()
