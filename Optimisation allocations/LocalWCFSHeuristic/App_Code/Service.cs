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
    // Method to return the allocations data from a heuristic algorithm.
    public AllocationsData GetHeuristicAllocations(ConfigData configData, int deadline)
	{
        // Stopwatch to check the operation time.
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        AllocationsData allocationsData = new AllocationsData();

        int numberOfProcessors = configData.NumberOfProcessors;
        int numberOfTasks = configData.NumberOfTasks;

        List<int[,]> allocationsList = new List<int[,]>();
        List<string> allocationsString = new List<string>();
        int[,] allocation = new int[numberOfProcessors, numberOfTasks];

        // Getting allocations from heuristic algorithm and adding to allocationsList.
        for (int i = 0; i < numberOfProcessors * numberOfProcessors; i++)
        {
            allocation = HeuristicAlgorithm(configData, allocationsData);

            if (allocation != null && !allocationsList.Contains(allocation))
            {
                allocationsList.Add(allocation);
            }
        }
        
        if (allocationsList.Count != 0)
        {
            // Converting the 2-D array of allocations to string.
            foreach (int[,] result in allocationsList)
            {
                allocationsString.Add(GetAllocationString(result, numberOfProcessors, numberOfTasks));
            }

            // Getting the data in Taff format.
            allocationsData.Description = GetTaffData(configData.FilePath, numberOfProcessors, numberOfTasks, allocationsString);
            allocationsData.Count = allocationsList.Count;

            // Checking whether the deadline is passed.
            if (stopwatch.ElapsedMilliseconds > deadline)
            {
                throw new TimeoutException("Heuristic Service operation timed out");
            }

            return (allocationsData);
        }

        // No optimal allocation found
        else
        {
            return (null);
        }    
	}

    // Method for the heuristic algorithm.
    private int[,] HeuristicAlgorithm(ConfigData configData, AllocationsData allocationsData)
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
                // Conerting 1-D arrays into 2-D arrays.
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
        int processornum = 0;
        double smallestRuntime;

        Random random = new Random();
        List<int> randomList = new List<int>();

        allocationsData.Energy = 99999D;

        do
        {
            // Randomly select a processor.
            row = random.Next(0, numberOfProcessors);
            
            // If random processor is already been searched.
            while (randomList.Contains(row))
            {
                row = random.Next(0, numberOfProcessors);
            }

            randomList.Add(row);

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
                    break;
                }

                // All the tasks are allocated.
                if (allocatedTasks == numberOfTasks)
                    break;
            }

            processornum++;

        } while (processornum < numberOfProcessors);

        double totalEnergy = Math.Round(allocationEnergies.Sum(), 3);

        // Assign the smallest energy.
        if (totalEnergy < allocationsData.Energy)
        {
            allocationsData.Energy = totalEnergy;
        }
       

        // Return a valid allocation.
        if (allocatedTasks == numberOfTasks)
        {
            return (allocation);
        }   
        else
        {
            return (null);
        }      
    }


    // Method to convert the 2-D allocation array to string.
    private string GetAllocationString(int[,] allocation, int processors, int tasks)
    {
        string result = "";

        for (int row = 0; row < processors; row++)
        {
            for (int column = 0; column < tasks; column++)
            {
                if (column > 0) result += ",";

                result += allocation[row, column];
            }

            if (row < processors - 1) result += ";";
        }

        return (result);
    }

    // Method to return the allocations data in the Taff file format.
    private string GetTaffData(string filename, int processors, int tasks, List<string> allocations)
    {
        string result = "";

        result += @"CONFIGURATION-DATA" + Environment.NewLine;
        result += @"FILENAME=" + @"""" + filename + @"""" + Environment.NewLine;
        result += @"END-CONFIGURATION-DATA" + Environment.NewLine;

        result += @"ALLOCATIONS" + Environment.NewLine;
        result += @"COUNT=" + allocations.Count + Environment.NewLine;
        result += @"TASKS=" + tasks.ToString() + Environment.NewLine;
        result += @"PROCESSORS=" + processors.ToString() + Environment.NewLine;

        for (int ID = 0; ID < allocations.Count; ID++)
        {
            result += @"ALLOCATION" + Environment.NewLine;
            result += @"ID=" + ID + Environment.NewLine;
            result += @"MAP=" + allocations[ID] + Environment.NewLine;
            result += @"END-ALLOCATION" + Environment.NewLine;
        }
        
        result += @"END-ALLOCATIONS" + Environment.NewLine;

        return (result);
    }
}
