using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammingTask1
{
    // Task class to manage the properties of each task and perform calculations.
    public class Task
    {
        // Properties.
        public int ID { get; set; }
        public double Runtime{ get; set; }
        public double ReferenceFrequency { get; set; }
        public int Ram { get; set; }
        public int DownloadSpeed { get; set; }
        public int UploadSpeed { get; set; }

        // Default Constructor.
        public Task()
        {

        }

        // Parameterised constructor.
        public Task(int id, double runtime, double referenceFrequency, int ram, int download, int upload)
        {
            ID = id;
            Runtime = runtime;
            ReferenceFrequency = referenceFrequency;
            Ram = ram;
            DownloadSpeed = download;
            UploadSpeed = upload;
        }

        // Method to check whether the task ram is sufficient with the processor ram.
        public Boolean IsRamSufficient(Processor processor)
        {
            return (Ram <= processor.Ram);
        }

        // Method to check whether the task download speed is sufficient with the processor download speed.
        public Boolean IsDownloadSufficient(Processor processor)
        {
            return (DownloadSpeed <= processor.DownloadSpeed);
        }

        // Method to check whether the task upload speed is sufficient with the processor upload speed.
        public Boolean IsUploadSufficient(Processor processor)
        {
            return (UploadSpeed <= processor.UploadSpeed);
        }

        // Method to calculate the task runtime on processor.
        public Double CalculateRuntime(Processor processor)
        {
            return Math.Round((Runtime * (ReferenceFrequency / processor.Frequency)) , 2);
        }

    }
}
