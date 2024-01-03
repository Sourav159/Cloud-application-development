using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfServiceLibrary;

public class Service : IService
{
    // Method to return the allocations data from a greedy algorithm.
	public AllocationsData GetAllocations(ConfigData configData, int deadline)
	{
        // Stopwatch to check the operation time.
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        AllocationsData allocationsData = new AllocationsData();

        int numberOfProcessors = configData.NumberOfProcessors;
        int numberOfTasks = configData.NumberOfTasks;

        // Getting the optimal allocation from greedy algorithm.
        int[,] allocation = GreedyAlgorithm(configData, allocationsData);

        // Converting the 2-D array of allocation to string.
        string allocationString = GetAllocationString(allocation, numberOfProcessors, numberOfTasks);

        // Getting the data in Taff format.
        allocationsData.Description = GetTaffData(configData.FilePath, numberOfProcessors, numberOfTasks, allocationString);
        allocationsData.Count = 1;

        // Checking whether the deadline is passed.
        if (stopwatch.ElapsedMilliseconds > deadline)
        {
            throw new TimeoutException("Greedy Service operation timed out");
        }

        return (allocationsData);
	}

    // Method for the greedy algorithm.
	private int[,] GreedyAlgorithm(ConfigData configData, AllocationsData allocationsData)
    {
		// Variables.
		int numberOfProcessors = configData.NumberOfProcessors;
		int numberOfTasks = configData.NumberOfTasks;
		double programDuration = configData.Duration;

		double[,] runTimes = new double[numberOfProcessors, numberOfTasks];
		double[,] energies = new double[numberOfProcessors, numberOfTasks];
		int[,] blockingArray = new int[numberOfProcessors, numberOfTasks];
        int row, column;

		for (row = 0; row < numberOfProcessors; row++)
		{
			for (column = 0; column < numberOfTasks; column++)
			{
                // Conerting 1-D arrays into 2-D arrays for greedy algorithm.
                runTimes[row, column] = configData.Runtimes[row * numberOfTasks + column];
				energies[row, column] = configData.Energies[row * numberOfTasks + column];

                // Populating the Blocking array.
                if (configData.ProcessorRAM[row] < configData.TaskRAM[column]
                    || configData.ProcessorDownloadSpeed[row] < configData.TaskDownloadSpeed[column]
                    || configData.ProcessorUploadSpeed[row] < configData.TaskUploadSpeed[column])
                {
                    blockingArray[row, column] = -1;
                }
            }
		}
     
		int[,] allocation = new int[numberOfProcessors, numberOfTasks];
		List<double> taskRunTimes = new List<double>();
		List<double> allocationEnergies = new List<double>();
		bool[] columnAllocated = new bool[numberOfTasks];
        int allocatedTasks = 0;
        int smallestRow = 0;
        int smallestColumn = 0;
        double smallestRuntime;
        
        for (row = 0; row < numberOfProcessors; row++)
        {
            // Sum of task runtimes is less than the program duration.
            while (taskRunTimes.Sum() <= programDuration)
            {
                smallestRuntime = 99999D;

                // Finding the task with smallest runtime.
                for (column = 0; column < numberOfTasks; column++)
                {
                    if (runTimes[row, column] < smallestRuntime && columnAllocated[column] != true && blockingArray[row, column] != -1)
                    {
                        smallestRuntime = runTimes[row, column];
                        smallestRow = row;
                        smallestColumn = column;
                    }
                }

                // No smallest runtime found.
                if (smallestRuntime == 99999D)
                {
                    taskRunTimes = new List<double>();
                    break;
                }

                // Add the smallest task run time for the processor[i].
                taskRunTimes.Add(smallestRuntime);

                if (taskRunTimes.Sum() <= programDuration)
                {
                    // Allocate the smallest task to processor[i].
                    allocation[smallestRow, smallestColumn] = 1;
                    columnAllocated[smallestColumn] = true;
                    allocatedTasks++;

                    // Populate the allocation energy list.
                    allocationEnergies.Add(energies[smallestRow, smallestColumn]);
                }

                // Processor is full.
                else
                {
                    taskRunTimes = new List<double>();
                    
                    if (smallestRow != numberOfProcessors - 1)
                    {
                        if (blockingArray[smallestRow + 1, smallestColumn] == -1)
                        {
                            smallestRow++;
                        }

                        // Allocate the smallest task to next processor with smallest frequency.
                        allocation[smallestRow + 1, smallestColumn] = 1;
                        taskRunTimes.Add(runTimes[smallestRow + 1, smallestColumn]);
                        columnAllocated[smallestColumn] = true;
                        allocatedTasks++;

                        // Populate the allocation energy list.
                        allocationEnergies.Add(energies[smallestRow + 1, smallestColumn]);
                    }
                  
                    break;
                }
            }

            // All the tasks are allocated.
            if (allocatedTasks == numberOfTasks)
                break;
        }

        // Total energy for the allocation.
        double totalEnergy = Math.Round(allocationEnergies.Sum(), 3);
        allocationsData.Energy = totalEnergy;

        return (allocation);
    }

    // Method to convert the 2-D allocation array to string.
    private string GetAllocationString(int[,] allocation, int processors, int tasks)
    {
        string result = "";

        for (int row = 0; row < processors; row++)
        {
            for (int column = 0; column < tasks; column++)
            {
                if (column > 0)
                {
                    result += ",";
                }

                result += allocation[row, column];
            }

            if (row < processors - 1) result += ";";
        }

        return (result);
    }

    // Method to return the allocation data in the Taff file format.
    private string GetTaffData(string filename, int processors, int tasks, string allocation)
    { 
        string result = "";

        result += @"CONFIGURATION-DATA" + Environment.NewLine;
        result += @"FILENAME=" + @"""" + filename + @"""" +  Environment.NewLine;
        result += @"END-CONFIGURATION-DATA" + Environment.NewLine;

        result += @"ALLOCATIONS" + Environment.NewLine;
        result += @"COUNT=1" + Environment.NewLine;
        result += @"TASKS=" + tasks.ToString() + Environment.NewLine;
        result += @"PROCESSORS=" + processors.ToString() + Environment.NewLine;

        result += @"ALLOCATION" + Environment.NewLine;
        result += @"ID=0" + Environment.NewLine;
        result += @"MAP=" + allocation + Environment.NewLine;
        result += @"END-ALLOCATION" + Environment.NewLine;

        result += @"END-ALLOCATIONS" + Environment.NewLine;

        return (result);
    }  
}
