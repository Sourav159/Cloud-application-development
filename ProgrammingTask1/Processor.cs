using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgrammingTask1
{
    // Processor class to manage the properties of each processor.
    public class Processor
    {
        // Properties.
        public int ID { get; set; }
        public string Type { get; set; }
        public double Frequency { get; set; }
        public int Ram { get; set; }
        public int DownloadSpeed { get; set; }
        public int UploadSpeed { get; set; }
        
        // Default Constructor.
        public Processor()
        {

        }

        // Parameterised constructor.
        public Processor(int id, string type, double frequency, int ram, int download, int upload)
        {
            ID = id;
            Type = type;
            Frequency = frequency;
            Ram = ram;
            DownloadSpeed = download;
            UploadSpeed = upload;
        }   

    }
}
