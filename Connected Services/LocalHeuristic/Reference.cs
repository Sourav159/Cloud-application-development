﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AllocationsApplication.LocalHeuristic {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LocalHeuristic.IService")]
    public interface IService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetHeuristicAllocations", ReplyAction="http://tempuri.org/IService/GetHeuristicAllocationsResponse")]
        WcfServiceLibrary.AllocationsData GetHeuristicAllocations(WcfServiceLibrary.ConfigData configData, int deadline);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IService/GetHeuristicAllocations", ReplyAction="http://tempuri.org/IService/GetHeuristicAllocationsResponse")]
        System.IAsyncResult BeginGetHeuristicAllocations(WcfServiceLibrary.ConfigData configData, int deadline, System.AsyncCallback callback, object asyncState);
        
        WcfServiceLibrary.AllocationsData EndGetHeuristicAllocations(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : AllocationsApplication.LocalHeuristic.IService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetHeuristicAllocationsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetHeuristicAllocationsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public WcfServiceLibrary.AllocationsData Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((WcfServiceLibrary.AllocationsData)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceClient : System.ServiceModel.ClientBase<AllocationsApplication.LocalHeuristic.IService>, AllocationsApplication.LocalHeuristic.IService {
        
        private BeginOperationDelegate onBeginGetHeuristicAllocationsDelegate;
        
        private EndOperationDelegate onEndGetHeuristicAllocationsDelegate;
        
        private System.Threading.SendOrPostCallback onGetHeuristicAllocationsCompletedDelegate;
        
        public ServiceClient() {
        }
        
        public ServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<GetHeuristicAllocationsCompletedEventArgs> GetHeuristicAllocationsCompleted;
        
        public WcfServiceLibrary.AllocationsData GetHeuristicAllocations(WcfServiceLibrary.ConfigData configData, int deadline) {
            return base.Channel.GetHeuristicAllocations(configData, deadline);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetHeuristicAllocations(WcfServiceLibrary.ConfigData configData, int deadline, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetHeuristicAllocations(configData, deadline, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public WcfServiceLibrary.AllocationsData EndGetHeuristicAllocations(System.IAsyncResult result) {
            return base.Channel.EndGetHeuristicAllocations(result);
        }
        
        private System.IAsyncResult OnBeginGetHeuristicAllocations(object[] inValues, System.AsyncCallback callback, object asyncState) {
            WcfServiceLibrary.ConfigData configData = ((WcfServiceLibrary.ConfigData)(inValues[0]));
            int deadline = ((int)(inValues[1]));
            return this.BeginGetHeuristicAllocations(configData, deadline, callback, asyncState);
        }
        
        private object[] OnEndGetHeuristicAllocations(System.IAsyncResult result) {
            WcfServiceLibrary.AllocationsData retVal = this.EndGetHeuristicAllocations(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetHeuristicAllocationsCompleted(object state) {
            if ((this.GetHeuristicAllocationsCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetHeuristicAllocationsCompleted(this, new GetHeuristicAllocationsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetHeuristicAllocationsAsync(WcfServiceLibrary.ConfigData configData, int deadline) {
            this.GetHeuristicAllocationsAsync(configData, deadline, null);
        }
        
        public void GetHeuristicAllocationsAsync(WcfServiceLibrary.ConfigData configData, int deadline, object userState) {
            if ((this.onBeginGetHeuristicAllocationsDelegate == null)) {
                this.onBeginGetHeuristicAllocationsDelegate = new BeginOperationDelegate(this.OnBeginGetHeuristicAllocations);
            }
            if ((this.onEndGetHeuristicAllocationsDelegate == null)) {
                this.onEndGetHeuristicAllocationsDelegate = new EndOperationDelegate(this.OnEndGetHeuristicAllocations);
            }
            if ((this.onGetHeuristicAllocationsCompletedDelegate == null)) {
                this.onGetHeuristicAllocationsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetHeuristicAllocationsCompleted);
            }
            base.InvokeAsync(this.onBeginGetHeuristicAllocationsDelegate, new object[] {
                        configData,
                        deadline}, this.onEndGetHeuristicAllocationsDelegate, this.onGetHeuristicAllocationsCompletedDelegate, userState);
        }
    }
}
