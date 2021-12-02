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
    // Configuration class to manage the properties of CFF file and validate the file format.
    class Configuration
    {
        // Properties.
        public string LogFilename { get; set; }
        private int MinTasks { get; set; }
        private int MaxTasks { get; set; }
        private int MinProcessors { get; set; }
        private int MaxProcessors { get; set; }
        private double MinProcessorFreq { get; set; }
        private double MaxProcessorFreq { get; set; }
        private int MinProcessorRam { get; set; }
        private int MaxProcessorRam { get; set; }
        private int MinProcessorDownSpeed { get; set; }
        private int MaxProcessorDownSpeed { get; set; }
        private int MinProcessorUpSpeed { get; set; }
        private int MaxProcessorUpSpeed { get; set; }
        public static double ProgramDuration{ get; set; }
        private int ProgramTasks { get; set; }
        private int ProgramProcessors{ get; set; }
        private string LocalCommunication { get; set; }
        private string RemoteCommunication { get; set; }
        public Boolean FileValid { get; set; }

        public static List<Task> Tasks { get; set; }
        public static List<Processor> Processors { get; set; }
        private List<ProcessorType> ProcessorTypes { get; set; }
        public List<string> Errors { get; set; }

        // Variables.
        Task task = new Task();
        Processor processor = new Processor();
        ProcessorType processorType = new ProcessorType();
      
        // Regular expressions
        private Regex limitsRegex = new Regex(@"^(MINIMUM-TASKS|MAXIMUM-TASKS|MINIMUM-PROCESSORS|MAXIMUM-PROCESSORS|MINIMUM-RAM|MAXIMUM-RAM|MINIMUM-DOWNLOAD|MAXIMUM-DOWNLOAD|MINIMUM-UPLOAD|MAXIMUM-UPLOAD)=\d+$");
        private Regex limitsFrequencyRegex = new Regex(@"^(MINIMUM-PROCESSOR-FREQUENCIES|MAXIMUM-PROCESSOR-FREQUENCIES)=\d+(\.\d+)$");
        private Regex dataRegex = new Regex(@"^(TASKS|PROCESSORS|ID|RAM|DOWNLOAD|UPLOAD)=\d+$");
        private Regex dataDecimalRegex = new Regex(@"^(DURATION|RUNTIME|REFERENCE-FREQUENCY|FREQUENCY)=\d+(\.\d+)$");
        private Regex stringRegex = new Regex(@"^(TYPE|NAME)=.+$");
        private Regex coefficientRegex = new Regex(@"^(C2|C1|C0)=(-)?\d+(\.\d+)?$");
        private Regex communicationMapRegex = new Regex(@"^MAP=(\d+|\d+\.\d+)(,(\d+|\d+\.\d+))+(;(\d+|\d+\.\d+)(,(\d+|\d+\.\d+))+)+$");

        /// <summary>
        /// Method to validate the CFF file and return the boolean value whether the file conform to the CFF file format or not.
        /// It processes the CFF file, extracts the properites and checks the format.
        /// </summary>
        /// 
        /// <param name="cffFilename">
        /// The name of the CFF file to be validated.
        /// </param>
        /// 
        /// <returns>
        /// A boolean value to specify whether the contents of the CFF file conform to the CFF format.
        /// </returns>
        public Boolean Validate(string cffFilename)
        {
            // Initialising properties.
            LogFilename = null;

            Errors = new List<string>();
            Errors.Add($"<h3>{ Path.GetFileName(cffFilename) + " - file errors:"}</h3>");
            
            Tasks = new List<Task>();
            Processors = new List<Processor>();
            ProcessorTypes = new List<ProcessorType>();
       
            FileValid = true;

            StreamReader streamReader = new StreamReader(cffFilename);

            while (!streamReader.EndOfStream)
            {
                // Removing the leading and trailing white spaces from the line.
                string line = streamReader.ReadLine().Trim();

                // Check for empty line.
                if (line.Length == Int32.Parse(Constants.Empty))
                {
                    // Skip.
                }

                // Check for comment.
                else if (line.StartsWith(Constants.Comment))
                {
                    // Skip.
                }

                // Check for Logfile.
                else if (line.StartsWith(CffKeywords.Logfile))
                {
                    FileValid = CheckLogfile(line, streamReader, cffFilename);
                }

                // Check for Limits.
                else if (line.StartsWith(CffKeywords.Limits))
                {
                    FileValid = CheckLimits(line, streamReader);
                }

                // Check for Program data.
                else if (line.StartsWith(CffKeywords.Program))
                {
                    FileValid = CheckProgramData(line, streamReader);
                }

                // Check for Tasks data.
                else if (line.Equals(CffKeywords.Tasks))
                {
                    FileValid = CheckTasks(line, streamReader);
                }

                // Check for Processors data.
                else if (line.Equals(CffKeywords.Processors))
                {
                    FileValid = CheckProcessors(line, streamReader);
                }

                // Check for Processor Types data.
                else if (line.Equals(CffKeywords.ProcessorTypes))
                {
                    FileValid = CheckProcessorTypes(line, streamReader);
                }

                // Check for Local communication data.
                else if (line.Equals(CffKeywords.LocalCommunication))
                {
                    FileValid = CheckLocalCommunication(line, streamReader);
                }

                // Check for Remote communication data.
                else if (line.Equals(CffKeywords.RemoteCommunication))
                {
                    FileValid = CheckRemoteCommunication(line, streamReader);
                }

                // Error.
                else
                {
                    Errors.Add($"<br>{"Invalid keyword " + line}<br>");                  
                    FileValid = false;
                }

            }

            streamReader.Close();

            return FileValid;
        }

        /// <summary>
        /// Method to check the contents of logfile keyword and extract the logfile name.
        /// </summary>
        /// 
        /// <param name="line">
        /// The current streamraeder line.
        /// </param>
        /// 
        /// <param name="streamReader">
        /// The steamreader object for reading the file.
        /// </param>
        /// 
        /// <param name="cffFilename">
        /// The CFF file name.
        /// </param>
        /// 
        /// <returns>
        /// A boolean value specifying whether the contents are in correct format.
        /// </returns>
        private Boolean CheckLogfile(string line, StreamReader streamReader, string cffFilename)
        {
            Boolean valid = true;

            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine().Trim();

                if (line.StartsWith(CffKeywords.Default))
                {
                    string[] data = line.Split(char.Parse(Constants.EqualSign));

                    if (data.Length == Int32.Parse(Constants.ExpectedSize))
                    {
                        LogFilename = data[1].Trim(char.Parse(Constants.DoubleQuotes));

                        // Filename for Logfile
                        if (LogFilename.Length > 0)
                        {
                            // Invalid characters in CffFilename.
                            if (LogFilename.IndexOfAny(Path.GetInvalidFileNameChars()) == -1)
                            {
                                string path = Path.GetDirectoryName(cffFilename);
                                LogFilename = path + Path.DirectorySeparatorChar + LogFilename;
                            }
                            else
                            {
                                Errors.Add($"<br>{ "Invalid characters for Logfile " + line}<br> ");
                                valid = false;
                            }

                        }
                        else
                        {
                            Errors.Add($"<br>{ "No filename for Logfile " + line}<br> ");
                            valid = false;
                        }

                    }
                    else
                    {
                        Errors.Add($"<br>{ "No value for keyword " + line}<br> ");
                        valid = false;
                    }
                }

                // End of block.
                else if(line.Equals(CffKeywords.EndLogfile))
                {
                    break;
                }

            }
            
            return valid;
        }

        /// <summary>
        /// Method to check the format of limits keyword and extract the data from them.
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
        /// A boolean value specifying whether the contents of limit keywords are in correct format.
        /// </returns>
        private Boolean CheckLimits(string line, StreamReader streamReader)
        {
            Boolean valid = true;

            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine().Trim();

                // Match limits data.
                if (limitsRegex.IsMatch(line))
                {
                    if (line.StartsWith(CffKeywords.MinTasks))
                    {
                        string[] minTasks = line.Split(char.Parse(Constants.EqualSign));
                        MinTasks = Int32.Parse(minTasks[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MaxTasks))
                    {
                        string[] maxTasks = line.Split(char.Parse(Constants.EqualSign));
                        MaxTasks = Int32.Parse(maxTasks[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MinProcessors))
                    {
                        string[] minProcessors = line.Split(char.Parse(Constants.EqualSign));
                        MinProcessors = Int32.Parse(minProcessors[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MaxProcessors))
                    {
                        string[] maxProcessors = line.Split(char.Parse(Constants.EqualSign));
                        MaxProcessors = Int32.Parse(maxProcessors[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MinRam))
                    {
                        string[] minRam = line.Split(char.Parse(Constants.EqualSign));
                        MinProcessorRam = Int32.Parse(minRam[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MaxRam))
                    {
                        string[] maxRam = line.Split(char.Parse(Constants.EqualSign));
                        MaxProcessorRam = Int32.Parse(maxRam[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MinDownSpeed))
                    {
                        string[] minDown = line.Split(char.Parse(Constants.EqualSign));
                        MinProcessorDownSpeed = Int32.Parse(minDown[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MaxDownSpeed))
                    {
                        string[] maxDown = line.Split(char.Parse(Constants.EqualSign));
                        MaxProcessorDownSpeed = Int32.Parse(maxDown[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MinUpSpeed))
                    {
                        string[] minUp = line.Split(char.Parse(Constants.EqualSign));
                        MinProcessorUpSpeed = Int32.Parse(minUp[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MaxUpSpeed))
                    {
                        string[] maxUp = line.Split(char.Parse(Constants.EqualSign));
                        MaxProcessorUpSpeed = Int32.Parse(maxUp[1]);
                    }
                }

                // Match limits decimal data.
                else if (limitsFrequencyRegex.IsMatch(line))
                {
                    if (line.StartsWith(CffKeywords.MinProcessorFreq))
                    {
                        string[] minProcessorFreq = line.Split(char.Parse(Constants.EqualSign));
                        MinProcessorFreq = Double.Parse(minProcessorFreq[1]);
                    }
                    else if (line.StartsWith(CffKeywords.MaxProcessorFreq))
                    {
                        string[] maxProcessorFreq = line.Split(char.Parse(Constants.EqualSign));
                        MaxProcessorFreq = Double.Parse(maxProcessorFreq[1]);
                    }
                }

                // End of limits block.
                else if (line.Equals(CffKeywords.EndLimits))
                {
                    break;
                }

                else
                {
                    Errors.Add($"<br>{"Invalid value for limit " + line}<br>");
                    valid = false;
                }
                            
            }
             
            return valid;
        }

        /// <summary>
        /// Method to check the format of program data and extract the data from them.
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
        /// A boolean value specifying whether the contents of program data are in correct format.
        /// </returns>
        private Boolean CheckProgramData(string line, StreamReader streamReader)
        {
            Boolean valid = true;

            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine().Trim();

                // Match decimal program data.
                if (dataDecimalRegex.IsMatch(line))
                {
                    if(line.StartsWith(CffKeywords.ProgramDuration))
                    {
                        string[] duration = line.Split(char.Parse(Constants.EqualSign));
                        ProgramDuration = Double.Parse(duration[1]);
                    }
                    
                }

                // Match program data.
                else if (dataRegex.IsMatch(line))
                {
                    if (line.StartsWith(CffKeywords.ProgramTasks))
                    {
                        string[] tasks = line.Split(char.Parse(Constants.EqualSign));
                        ProgramTasks = Int32.Parse(tasks[1]);
                    }
                    else if (line.StartsWith(CffKeywords.ProgramProcessors))
                    {
                        string[] processors = line.Split(char.Parse(Constants.EqualSign));
                        ProgramProcessors = Int32.Parse(processors[1]);
                    }
                }

                // End of program block.
                else if (line.Equals(CffKeywords.EndProgram))
                {
                    break;
                }

                else
                {
                    Errors.Add($"<br>{"Invalid value for keyword " + line}<br>");
                    valid = false;
                }
            }
            
            return valid;
        }

        /// <summary>
        /// Method to check the format of tasks data and extract the data from them.
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
        /// A boolean value specifying whether the contents of tasks data are in correct format.
        /// </returns>
        private Boolean CheckTasks(string line, StreamReader streamReader)
        {
            Boolean valid = true;
            
            while (!streamReader.EndOfStream)
            {
                string taskLine = streamReader.ReadLine().Trim();

                if (taskLine.Equals(CffKeywords.Task))
                {
                    // Skip.
                }

                // Match tasks data.
                else if (dataRegex.IsMatch(taskLine))
                {

                    if (taskLine.StartsWith(CffKeywords.ID))
                    {
                        string[] id = taskLine.Split(char.Parse(Constants.EqualSign));
                        task.ID = Int32.Parse(id[1]);
                    }
                    else if (taskLine.StartsWith(CffKeywords.Ram))
                    {
                        string[] ram = taskLine.Split(char.Parse(Constants.EqualSign));
                        task.Ram = Int32.Parse(ram[1]);
                    }
                    else if (taskLine.StartsWith(CffKeywords.DownSpeed))
                    {
                        string[] download = taskLine.Split(char.Parse(Constants.EqualSign));
                        task.DownloadSpeed = Int32.Parse(download[1]);
                    }
                    else if (taskLine.StartsWith(CffKeywords.UpSpeed))
                    {
                        string[] upload = taskLine.Split(char.Parse(Constants.EqualSign));
                        task.UploadSpeed = Int32.Parse(upload[1]);
                    }
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + taskLine}<br>");
                        valid = false;
                    }
                }

                // Match decimal tasks data.
                else if (dataDecimalRegex.IsMatch(taskLine))
                {
                    if (taskLine.StartsWith(CffKeywords.Runtime))
                    {
                        string[] runtime = taskLine.Split(char.Parse(Constants.EqualSign));

                        task.Runtime = Double.Parse(runtime[1]);
                    }
                    else if (taskLine.StartsWith(CffKeywords.ReferenceFreq))
                    {
                        string[] referenceFreq = taskLine.Split(char.Parse(Constants.EqualSign));

                        task.ReferenceFrequency = Double.Parse(referenceFreq[1]);
                    }
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + taskLine}<br>");
                        valid = false;
                    }
                }

                // End of single task -  Populate the tasks list.
                else if (taskLine.Equals(CffKeywords.EndSingleTask))
                {
                    Tasks.Add(task);
                    task = new Task();
                }

                // End of all tasks block
                else if (taskLine.Equals(CffKeywords.EndAllTasks))
                {
                    break;
                }
            }

            return valid;
        }

        /// <summary>
        /// Method to check the format of processors data and extract the data from them.
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
        /// A boolean value specifying whether the contents of processors data are in correct format.
        /// </returns>
        private Boolean CheckProcessors(string line, StreamReader streamReader)
        {
            Boolean valid = true;

            while (!streamReader.EndOfStream)
            {
                string processorLine = streamReader.ReadLine().Trim();

                if (processorLine.Equals(CffKeywords.Processor))
                {
                    // Skip.
                }

                // Match processor data
                else if (dataRegex.IsMatch(processorLine))
                {

                    if (processorLine.StartsWith(CffKeywords.ID))
                    {
                        string[] id = processorLine.Split(char.Parse(Constants.EqualSign));
                        processor.ID = Int32.Parse(id[1]);
                    }
                    else if (processorLine.StartsWith(CffKeywords.Ram))
                    {
                        string[] ram = processorLine.Split(char.Parse(Constants.EqualSign));
                        processor.Ram = Int32.Parse(ram[1]);
                    }
                    else if (processorLine.StartsWith(CffKeywords.DownSpeed))
                    {
                        string[] download = processorLine.Split(char.Parse(Constants.EqualSign));
                        processor.DownloadSpeed = Int32.Parse(download[1]);
                    }
                    else if (processorLine.StartsWith(CffKeywords.UpSpeed))
                    {
                        string[] upload = processorLine.Split(char.Parse(Constants.EqualSign));
                        processor.UploadSpeed = Int32.Parse(upload[1]);
                    }
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + processorLine}<br>");
                        valid = false;
                    }
                }

                // Match decimal processor data.
                else if (dataDecimalRegex.IsMatch(processorLine))
                {
                    if (processorLine.StartsWith(CffKeywords.Frequency))
                    {
                        string[] frequency = processorLine.Split(char.Parse(Constants.EqualSign));

                        processor.Frequency = Double.Parse(frequency[1]);
                    }                 
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + processorLine}<br>");
                        valid = false;
                    }
                }

                // Match string processor data.
                else if (stringRegex.IsMatch(processorLine))
                {
                    if (processorLine.StartsWith(CffKeywords.Type))
                    {
                        string[] type = processorLine.Split(char.Parse(Constants.EqualSign));
                        processor.Type = type[1].Trim(char.Parse(Constants.DoubleQuotes));                    
                    }
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + processorLine}<br>");
                        valid = false;
                    }
                }

                // End of single processor -  Populate the Processors list.
                else if (processorLine.Equals(CffKeywords.EndProcessor))
                {
                    Processors.Add(processor);
                    processor = new Processor();
                }

                // End of all processors block.
                else if (processorLine.Equals(CffKeywords.EndProcessors))
                {
                    break;
                }
            }

            return valid;
        }

        /// <summary>
        /// Method to check the format of processor types data and extract the data from them.
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
        /// A boolean value specifying whether the contents of processor types data are in correct format.
        /// </returns>
        private Boolean CheckProcessorTypes(string line, StreamReader streamReader)
        {
            Boolean valid = true;

            while (!streamReader.EndOfStream)
            {
                string processorLine = streamReader.ReadLine().Trim();

                if (processorLine.Equals(CffKeywords.ProcessorType))
                {
                    // Skip.
                }

                // Match string data.
                else if (stringRegex.IsMatch(processorLine))
                {

                    if (processorLine.StartsWith(CffKeywords.Name))
                    {
                        string[] name = processorLine.Split(char.Parse(Constants.EqualSign));
                        processorType.Name = name[1].Trim(char.Parse(Constants.DoubleQuotes));
                    }
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + processorLine}<br>");
                        valid = false;
                    }
                }

                // Match coefficient data.
                else if (coefficientRegex.IsMatch(processorLine))
                {
                    if (processorLine.StartsWith(CffKeywords.C2))
                    {
                        string[] c2 = processorLine.Split(char.Parse(Constants.EqualSign));
                        processorType.C2 = double.Parse(c2[1]);
                    }
                    else if (processorLine.StartsWith(CffKeywords.C1))
                    {
                        string[] c1 = processorLine.Split(char.Parse(Constants.EqualSign));
                        processorType.C1 = double.Parse(c1[1]);
                    }
                    else if (processorLine.StartsWith(CffKeywords.C0))
                    {
                        string[] c0 = processorLine.Split(char.Parse(Constants.EqualSign));
                        processorType.C0= double.Parse(c0[1]);
                    }
                    else
                    {
                        Errors.Add($"<br>{"Invalid value for keyword " + processorLine}<br>");
                        valid = false;
                    }
                }

                // End of single processor type -  Populate the Processor Types list.
                else if (processorLine.Equals(CffKeywords.EndProcessorType))
                {
                    ProcessorTypes.Add(processorType);
                    processorType = new ProcessorType();
                }

                // End of all processor types block.
                else if (processorLine.Equals(CffKeywords.EndProcessorTypes))
                {
                    break;
                }
            }

            return valid;
        }

        /// <summary>
        /// Method to check the format of local communication data and extract the data from them.
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
        /// A boolean value specifying whether the contents of local communication data are in correct format.
        /// </returns>
        private Boolean CheckLocalCommunication(string line, StreamReader streamReader)
        {
            Boolean valid = true;

            while (!streamReader.EndOfStream)
            {
                string local = streamReader.ReadLine().Trim();

                // Match communication data.
                if (communicationMapRegex.IsMatch(local))
                {
                    string[] map = local.Split(char.Parse(Constants.EqualSign));
                    LocalCommunication = map[1];
                }

                // End of local communication block.
                else if (local.Equals(CffKeywords.EndLocalCommunication))
                {
                    break;
                }

                else
                {
                    Errors.Add($"<br>{"Invalid value for keyword " + local}<br>");
                    valid = false;
                }                        
            }

            return valid;
        }

        /// <summary>
        /// Method to check the format of remote communication data and extract the data from them.
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
        /// A boolean value specifying whether the contents of remote communication data are in correct format.
        /// </returns>
        private Boolean CheckRemoteCommunication(string line, StreamReader streamReader)
        {
            Boolean valid = true;

            while (!streamReader.EndOfStream)
            {
                string remote = streamReader.ReadLine().Trim();

                // Match communication data.
                if (communicationMapRegex.IsMatch(remote))
                {
                    string[] map = remote.Split(char.Parse(Constants.EqualSign));
                    RemoteCommunication = map[1];
                }

                // End of remote communicaation block.
                else if (remote.Equals(CffKeywords.EndRemoteCommunication))
                {
                    break;
                }

                else
                {
                    Errors.Add($"<br>{"Invalid value for keyword " + remote}<br>");
                    valid = false;
                }
            }

            return valid;
        }
    }
}
