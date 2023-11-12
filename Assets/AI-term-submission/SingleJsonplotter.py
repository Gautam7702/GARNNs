import json
import numpy as np
import matplotlib.pyplot as plt

# Define the path to the JSON file
file_path = r"C:\Users\gomci\OneDrive\Desktop\AI-term-paper\Output_ANN\0-8-3-25-GA.json"

# Read the JSON data from the file
try:
    with open(file_path, "r") as json_file:
        json_data = json.load(json_file)

    # Extract relevant information
    num_generations = json_data["numberOfGenerations"]
    best_agent_selection = json_data["bestAgentSelection"]
    worst_agent_selection = json_data["worstAgentSelection"]
    numberToCrossover = json_data["numberToCrossover"]
    mutationRate = json_data["mutationRate"]
    fitness_values = json_data["fitnessAccrossGen"]

    # Create an array for generation numbers
    generation_numbers = np.arange(1, num_generations + 1)

    # Create the graph without lines
    fig, ax = plt.subplots(figsize=(10, 6))
    ax.plot(generation_numbers, fitness_values, marker='o', markersize=4, color='blue')
    ax.title.set_text("Fitness Change Across Generations")
    ax.set_xlabel("Generation")
    ax.set_ylabel("Fitness")
    
    # Annotate other parameters at the top of the plot
    ax.text(0.01, 0.95, f"Best Agent Selection: {best_agent_selection}", transform=ax.transAxes)
    ax.text(0.01, 0.9, f"Worst Agent Selection: {worst_agent_selection}", transform=ax.transAxes)
    ax.text(0.01, 0.85, f"Number of Crossover: {numberToCrossover}", transform=ax.transAxes)
    ax.text(0.01, 0.80, f"Mutation rate: {mutationRate}", transform=ax.transAxes)

    # Show the graph
    ax.grid(True)
    plt.show()

except FileNotFoundError:
    print(f"File not found: {file_path}")
except json.JSONDecodeError as e:
    print(f"Error decoding JSON: {e}")
except KeyError as e:
    print(f"Key not found: {e}")