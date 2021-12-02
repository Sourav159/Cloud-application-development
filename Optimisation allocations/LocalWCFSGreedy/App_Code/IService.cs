using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using WcfServiceLibrary;

[ServiceContract]
public interface IService
{
	[OperationContract]
	AllocationsData GetAllocations(ConfigData config, int deadline);
}

