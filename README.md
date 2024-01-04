# Cloud application development - Task Allocation Service
The Task Allocation Service is a set of WCF (Windows Communication Foundation) services that provide task allocations based on either a Greedy or a Heuristic algorithm connected to an Amazon Web Services (AWS) architecture. The service allows clients to request task allocations for a given configuration and deadline. This README provides an overview of the project structure and usage.

## Introduction
The service is implemented using WCF and includes two algorithms for task allocation: Greedy Algorithm and Heuristic Algorithm. Both algorithms aim to allocate tasks to processors while considering various constraints such as runtime, energy, and communication speed.

## Greedy Algorithm
The Greedy Algorithm uses a greedy approach to allocate tasks to processors. It calculates the optimal allocation based on task runtimes, energies, and various constraints. The algorithm stops if the deadline is exceeded.

## Heuristic Algorithm
The Heuristic Algorithm provides multiple allocations using a random heuristic approach. It randomly selects processors and attempts to allocate tasks while considering constraints. The algorithm iterates through multiple attempts and returns the allocations that meet the criteria.

### Watch a screencast of the application demonstration [here](https://youtu.be/aQVy1hK76po).

## Usage
### ConfigData Class:
ConfigData class contains the necessary configuration data for task allocation, such as the number of processors, tasks, task runtimes, energies, and various constraints.

## Greedy Algorithm Service
### GetAllocations: 
Returns task allocations using the Greedy Algorithm.

### Parameters:
### ConfigData: 
Configuration data for task allocation.

### Deadline: 
Deadline for the operation.

## Heuristic Algorithm Service:
### GetHeuristicAllocations:
Returns multiple task allocations using the Heuristic Algorithm.

### Parameters:
ConfigData: Configuration data for task allocation.
Deadline: Deadline for the operation.

## Exception Handling:
The services include basic exception handling to manage errors gracefully. If the operation takes longer than the specified deadline, a TimeoutException is thrown.

### Feel free to explore and use the Task Allocation Service to optimize task allocations based on your requirements.





