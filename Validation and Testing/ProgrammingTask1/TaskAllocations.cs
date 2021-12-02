using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]

namespace ProgrammingTask1
{
    // Class to manage the properties of TAFF file and validate the file format.
    class TaskAllocations
    {
        // Properties.
        public string CffFilename { get; set; }
        public int NumberOfAllocations { get; set; }
        public static int NumberOfTasks { get; set; }
        public static int NumberOfProcessors { get; set; }
        public static string Result { get; set; }

        public List<Allocation> Allocations { get; set; }
        public List<string> Errors { get; set; }
        

        // Variables
        string taffFileName;
        bool fileValid;
        Allocation allocation = new Allocation();
          
        // Regular Expressions
        private Regex allocationDataRegex = new Regex(@"^COUNT|TASKS|PROCESSORS=\d+$");
        private Regex allocationIdRegex = new Regex(@"^ID=\d+$");
        private Regex allocationMapRegex = new Regex(@"^MAP=([0|1][,0|,1]+)(;[0|1][,0|,1]+)+$");

        // Parameterised Constructor
        public TaskAllocations(string fileName)
        {
            taffFileName = fileName;

            Errors = new List<string>();
            Allocations = new List<Allocation>();        
        }
        /// <summary>
        /// Method to check the contents of Filename keyword and extract the CFF filename.
        /// </summary>
        /// 
        /// <returns>
        /// A boolean value specifying whether the content is in correct format.
        /// </returns>
        public Boolean GetCffFilename()
        {
            CffFilename = null;        
            StreamReader streamReader = new StreamReader(taffFileName);

            while (!streamReader.EndOfStream)
            { 
                string line = streamReader.ReadLine();
                line = line.Trim();

                if (line.StartsWith(TaffKeywords.FileName))
                {
                    string[] data = line.Split(char.Parse(Constants.EqualSign));

                    if (data.Length == Int32.Parse(Constants.ExpectedSize))
                    {
                        CffFilename = data[1].Trim(char.Parse(Constants.DoubleQuotes));

                        // CFF filename found.
                        if (CffFilename.Length > 0)
                        {
                            // Invalid characters in CffFilename
                            if (CffFilename.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                            {
                                string path = Path.GetDirectoryName(taffFileName);
                                CffFilename = path + Path.DirectorySeparatorChar + CffFilename;
                            }
                            else
                            {
                                Errors.Add($"<br>{"Invalid characters in Cff filename"}<br>" );
                            }

                        }
                        else
                        {
                            Errors.Add($"<br>{"No Cff filename found"}<br>" );
                        }

                    }
                    else
                    {
                        Errors.Add($"<br>{"No value for keyword"}<br>");
                    }

                }

            }

            streamReader.Close();
            return (Errors.Count == 0);
        }

        /// <summary>
        /// Method to validate the TAFF file and return the boolean value whether the file conform to the TAFF file format or not.
        /// It processes the TAFF file, extracts the properites and checks the format.
        /// </summary>
        /// 
        /// <returns>
        /// A boolean value to specify whether the contents of the TAFF file conform to the TAFF format.
        /// </returns>
        public Boolean Validate()
        {
            fileValid = true;

            Errors.Add($"<h3>{Path.GetFileName(taffFileName) + " - file errors:"}</h3>");

            // Process file.
            StreamReader streamReader = new StreamReader(taffFileName);

            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine().Trim();

                // Check for empty line.
                if (line.Length == Int32.Parse(Constants.Empty))
                {
                    // Skip.
                }
                else if (line.StartsWith(Constants.Comment))
                {
                    // Skip.
                }
                else if (line.StartsWith(Constants.TripleComment))
                {
                    // Skip.
                }
                else if (line.StartsWith(TaffKeywords.ConfigurationData))
                {
                    // Skip.
                }
                else if (line.StartsWith(TaffKeywords.FileName))
                {
                    // Skip.
                }
                else if (line.StartsWith(TaffKeywords.EndConfiguration))
                {
                    // Skip.
                }

                // Process the Allocations block
                else if (line.StartsWith(TaffKeywords.Allocations))
                {
                    fileValid = CheckAllocations(line, streamReader);
                }

                // Error.
                else
                {
                    Errors.Add($"<br>{"Invalid keyword " + line}<br>");
                    fileValid = false;

                }

            }

            streamReader.Close();

            return fileValid;
        }

        /// <summary>
        /// Method to check the format of allocations data and extract the data from them.
        /// </summary>
        /// 
        /// <param name="line">
        /// The current streamreader line.
        /// </param>
        /// 
        /// <param name="streamReader">
        /// Streamreader object to read the file.
        /// </param>
        /// 
        /// <returns>
        /// A boolean value specifying whether the contents of allocations data are in correct format.
        /// </returns>
        private Boolean CheckAllocations(string line, StreamReader streamReader)
        {
            Boolean valid = true;

            // Process the Allocations block till end of Allocations.
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine().Trim();

                // Match allocation data.
                if (allocationDataRegex.IsMatch(line))
                {
                    if (line.StartsWith(TaffKeywords.Count))
                    {
                        string[] count = line.Split(char.Parse(Constants.EqualSign));
                        NumberOfAllocations = Int32.Parse(count[1]);
                    }
                    else if (line.StartsWith(TaffKeywords.Tasks))
                    {
                        string[] tasks = line.Split(char.Parse(Constants.EqualSign));
                        NumberOfTasks = Int32.Parse(tasks[1]);
                    }
                    else if (line.StartsWith(TaffKeywords.Processors))
                    {
                        string[] processors = line.Split(char.Parse(Constants.EqualSign));
                        NumberOfProcessors = Int32.Parse(processors[1]);
                    }
                }
                else if (line.Equals(TaffKeywords.Allocation))
                {
                    // Skip.
                }

                // Match Allocation ID
                else if (line.StartsWith(TaffKeywords.ID))
                {
                    if (allocationIdRegex.IsMatch(line))
                    {
                        string[] id = line.Split(char.Parse(Constants.EqualSign));
                        allocation.AllocationID = Int32.Parse(id[1]);

                        // Allocations more than expected number
                        if (Allocations.Count >= NumberOfAllocations)
                        {
                            Errors.Add($"<br>{"More than expected number of allocations of " + NumberOfAllocations + " at " + line}<br>");
                            valid = false;
                        }
                    }
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + line}<br>");
                        valid = false;
                    }
                }

                // Match Allocation Map
                else if (line.StartsWith(TaffKeywords.Map))
                {
                    if (allocationMapRegex.IsMatch(line))
                    {
                        string[] map = line.Split(char.Parse(Constants.EqualSign));
                        allocation.AllocationMap = map[1];
                    }
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + line}<br>");
                        valid = false;
                    }
                }

                // End of single allocation -  Populate the Allocations list.
                else if (line.Equals(TaffKeywords.EndAllocation))
                {
                    Allocations.Add(allocation);
                    allocation = new Allocation();
                }

                // End of all allocations block.
                else if (line.Equals(TaffKeywords.EndAllocations))
                {
                    break;
                }

            }

            // Validating the format of each allocation in Allocations list and combining the errors list.
            foreach (Allocation allocation in Allocations)
            {
                List<string> allocationErrors = allocation.ValidateFormat();
                Errors.AddRange(allocationErrors);
            }

            return valid;
        }

        /// <summary>
        /// Method to diplay the final output to the GUI.
        /// It displays the TAFF and CFF file valid and calls allocation method to display the information for each allocation.
        /// </summary>
        /// 
        /// <param name="CffFileValid">
        /// Boolean value to represent whther CFF file is valid or not.
        /// </param>
        /// 
        /// <param name="TaffFileValid">
        /// Boolean value to represent whther TAFF file is valid or not.
        /// </param>
        /// 
        /// <returns>
        /// A string of output data to be displayed on the main form of GUI.
        /// </returns>
        public string ShowAllocations(Boolean CffFileValid, Boolean TaffFileValid)
        {
            Result = null;

            if (TaffFileValid == true)
            {
                Result += $"<p>TAFF File ({Path.GetFileName(taffFileName)}) is Valid.</p>";
            }
            else
            {
                Result += $"<p>TAFF File ({Path.GetFileName(taffFileName)}) is Invalid.</p>";
            }

            if (CffFileValid == true)
            {
                Result += $"<p>CFF File ({Path.GetFileName(CffFilename)}) is Valid.</p>";
            }
            else
            {
                Result += $"<p>CFF File ({Path.GetFileName(CffFilename)}) is Invalid.</p>";
            }

            // Combining result for each allocation.
            foreach (Allocation allocation in Allocations)
            {
                Result += allocation.ShowOutput();
            }

            return Result;
        }

    }
}