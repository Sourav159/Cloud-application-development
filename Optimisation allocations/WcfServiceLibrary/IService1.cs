using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfServiceLibrary
{
    // Data Contract for passing the Configuration data to WCFS.
    [DataContract]
    public class ConfigData
    {
        [DataMember]
        public double Duration { get; set; }

        [DataMember]
        public int NumberOfTasks { get; set; }

        [DataMember]
        public int NumberOfProcessors { get; set; }

        [DataMember]
        public double[] Energies { get; set; }

        [DataMember]
        public double[] Runtimes { get; set; }

        [DataMember]
        public int[] ProcessorUploadSpeed { get; set; }

        [DataMember]
        public int[] ProcessorDownloadSpeed { get; set; }

        [DataMember]
        public int[] ProcessorRAM { get; set; }

        [DataMember]
        public int[] TaskUploadSpeed { get; set; }

        [DataMember]
        public int[] TaskDownloadSpeed { get; set; }

        [DataMember]
        public int[] TaskRAM { get; set; }

        [DataMember]
        public string FilePath { get; set; }

        [DataMember]
        public double[] LocalCommunication { get; set; }

        [DataMember]
        public double[] RemoteCommunication { get; set; }
    }

    // Data Contract for passing the Allocations data from WCFS to client.
    [DataContract]
    public class AllocationsData
    {
        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public double Energy { get; set; }

        [DataMember]
        public int Count { get; set; }
    }

}
