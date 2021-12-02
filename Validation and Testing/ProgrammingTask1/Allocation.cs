using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammingTask1
{
    // Allocation class to manage the properties for each allocation and perform the validations for format and allocations.
    class Allocation
    {
        // Properties
        public int AllocationID { get; set; }
        public string AllocationMap { get; set; }
        public string[] MapData { get; set; }
        public List<string> AllocationErrors { get; set; }

        // Variables
        private bool[] taskAllocated;
        
        List<double> processorRuntime;
        List<int> processorMaxRam;
        List<int> processorMaxUpload;
        List<int> processorMaxDownload;

        int taskMaxRam;
        int taskMaxUpload;
        int taskMaxDownload;
        double taskRuntime;
        double allocationRuntime;
        bool allocationValid;

        /// <summary>
        /// Method to return the list of allocation errors that are specific to the format of allocations.
        /// It detects the allocations format errors and also performs calculations for allocation specific properties.
        /// </summary>
        /// 
        /// <returns>
        /// A list of errors that contains the information about the format of the allocations.
        /// </returns>
        public List<string> ValidateFormat()
        {
            // Initially assuming the allocation is valid.
            allocationValid = true;

            // Initialsing variables and property.
            taskAllocated = new bool[TaskAllocations.NumberOfTasks];
            processorMaxRam = new List<int>();
            processorMaxUpload = new List<int>();
            processorMaxDownload = new List<int>();
            processorRuntime = new List<double>();

            AllocationErrors = new List<string>();

            // Storing the Map data in an array.
            MapData = AllocationMap.Split(Convert.ToChar(Constants.SemiColon));

            if(MapData.Length != TaskAllocations.NumberOfProcessors)
            {
                AllocationErrors.Add($"<br>{"Invalid number of map sections " + MapData.Length + " for Allocation ID " + AllocationID}<br>");
                allocationValid = false;             
            }

            try
            {
                // Processing each allocation in Map data.
                foreach (string str in MapData)
                {             
                    int i = 0;
                    taskMaxRam = 0;
                    taskMaxDownload = 0;
                    taskMaxUpload = 0;
                    taskRuntime = 0;                 
          
                    int processorIndex = Array.IndexOf(MapData, str);
                    
                    string data = str.Replace(Constants.CommaSign, string.Empty);

                    if (data.Length != TaskAllocations.NumberOfTasks)
                    {
                        AllocationErrors.Add($"<br>{"Invalid number of tasks " + data.Length + " for Allocation ID " + AllocationID}<br>");
                        allocationValid = false;
                    }
                          
                    // Processing each task in the allocation.
                    foreach (char tasks in data)
                    {       
                        // Task is allocated to processor.
                        if (Char.ToString(tasks).Equals(Constants.TaskAllocated))
                        {
                            int taskIndex = data.IndexOf(Constants.TaskAllocated, i);

                            // Task already allocated.
                            if (taskAllocated[taskIndex] == true)
                            {
                                AllocationErrors.Add($"<br>{"Task " + (taskIndex + 1) + " is allocated to more than one processor for Allocation ID " + AllocationID}<br>");
                                allocationValid = false;
                            }
                            else
                            {
                                // Allocate the task.
                                taskAllocated[taskIndex] = true;
                                i = taskIndex + 1;

                                // Calculating task properties.
                                CalculateMaxRam(taskIndex);
                                CalculateMaxDownload(taskIndex);
                                CalculateMaxUpload(taskIndex);

                                taskRuntime += Configuration.Tasks[taskIndex].CalculateRuntime(Configuration.Processors[processorIndex]);                                
                            }

                        }
                       
                    }
             
                    // Adding task properites to the respective processor.
                    processorMaxRam.Add(taskMaxRam);
                    processorMaxDownload.Add(taskMaxDownload);
                    processorMaxUpload.Add(taskMaxUpload);                  
                    processorRuntime.Add(taskRuntime);
                }

                //Checking if all the tasks in the allocation are allocated.  
                for (int j = 0; j < TaskAllocations.NumberOfTasks; j++)
                {
                    if (taskAllocated[j] == false)
                    {
                        AllocationErrors.Add($"<br>{"Task " + (j + 1) + " is not allocated to to any processor for Allocation ID " + AllocationID}<br>");
                        allocationValid = false;
                    }
                }

                // Calculating the allocation runtime.
                allocationRuntime = processorRuntime.Max();
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            
            return AllocationErrors;
        }

        // Method to calculate the maximum ram for the tasks.
        private void CalculateMaxRam(int position)
        {
            if (Configuration.Tasks[position].Ram > taskMaxRam)
            {
                taskMaxRam = Configuration.Tasks[position].Ram;
            }          
        }

        // Method to calculate the maximum download speed for the tasks.
        private void CalculateMaxDownload(int position)
        {
            if (Configuration.Tasks[position].DownloadSpeed > taskMaxDownload)
            {
                taskMaxDownload = Configuration.Tasks[position].DownloadSpeed;
            }
        }

        // Method to calculate the maximum upload speed for the tasks.
        private void CalculateMaxUpload(int position)
        {
            if (Configuration.Tasks[position].UploadSpeed > taskMaxUpload)
            {
                taskMaxUpload = Configuration.Tasks[position].UploadSpeed;
            }
        }

        /// <summary>
        /// Method to validate the allocations for valid TAFF and CFF files.
        /// It checks the logic of allocations between tasks and processors and populate the errors list.
        /// </summary>
        /// 
        /// <returns>
        /// A list of errors that contains information about the irregulaties in the allocation data set.
        /// </returns>
        public List<string> ValidateAllocation()
        {
            if (allocationRuntime > Configuration.ProgramDuration)
            {
                AllocationErrors.Add($"<br>Runtime ({allocationRuntime}) of allocation (ID={AllocationID}) is more than expected program runtime ({Configuration.ProgramDuration})<br>");
            }

            for (int i = 0; i < TaskAllocations.NumberOfProcessors; i++)
            {    
                if (processorMaxRam[i] > Configuration.Processors[i].Ram)
                {
                    AllocationErrors.Add($"<br>Processor (ID = {i}) of allocation (ID={AllocationID}) has {Configuration.Processors[i].Ram} GB RAM but requires {processorMaxRam[i]} GB RAM.<br>");
                }

                if (processorMaxDownload[i] > Configuration.Processors[i].DownloadSpeed)
                {
                    AllocationErrors.Add($"<br>Processor (ID = {i}) of allocation (ID={AllocationID}) has {Configuration.Processors[i].DownloadSpeed} Gbps Download Speed but requires {processorMaxDownload[i]} Gbps.<br>");
                }

                if (processorMaxUpload[i] > Configuration.Processors[i].UploadSpeed)
                {
                    AllocationErrors.Add($"<br>Processor (ID = {i}) of allocation (ID={AllocationID}) has {Configuration.Processors[i].UploadSpeed} Gbps Upload Speed but requires {processorMaxUpload[i]} Gbps.<br>");
                }
            }

            return AllocationErrors;
        }

        /// <summary>
        /// Method to return the string output to display the information about each allocation.
        /// It displays the output for Allocation runtime, each allocation data set, ram, download speed, upload speed requirements.
        /// </summary>
        /// 
        /// <returns>
        /// A string of output data that contains the information about each allocation.
        /// </returns>
        public string ShowOutput()
        {
            string output = $"<p>Allocation ID = {AllocationID} , Runtime = {allocationRuntime}</p>";

            try
            {     
                for (int i = 0; i < TaskAllocations.NumberOfProcessors; i++)
                {
                    output += $"<p>{MapData[i]} &nbsp; {processorMaxRam[i]}/{Configuration.Processors[i].Ram} GB ";
                    output += $" &nbsp; {processorMaxDownload[i]}/{Configuration.Processors[i].DownloadSpeed} Gbps ";
                    output += $" &nbsp; {processorMaxUpload[i]}/{Configuration.Processors[i].UploadSpeed} Gbps </p>";
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return output;        
        }

    }
}
