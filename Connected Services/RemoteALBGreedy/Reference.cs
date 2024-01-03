﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AllocationsApplication.RemoteALBGreedy {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RemoteALBGreedy.IService")]
    public interface IService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService/GetAllocations", ReplyAction="http://tempuri.org/IService/GetAllocationsResponse")]
        WcfServiceLibrary.AllocationsData GetAllocations(WcfServiceLibrary.ConfigData config, int deadline);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IService/GetAllocations", ReplyAction="http://tempuri.org/IService/GetAllocationsResponse")]
        System.IAsyncResult BeginGetAllocations(WcfServiceLibrary.ConfigData config, int deadline, System.AsyncCallback callback, object asyncState);
        
        WcfServiceLibrary.AllocationsData EndGetAllocations(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceChannel : AllocationsApplication.RemoteALBGreedy.IService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetAllocationsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetAllocationsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
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
    public partial class ServiceClient : System.ServiceModel.ClientBase<AllocationsApplication.RemoteALBGreedy.IService>, AllocationsApplication.RemoteALBGreedy.IService {
        
        private BeginOperationDelegate onBeginGetAllocationsDelegate;
        
        private EndOperationDelegate onEndGetAllocationsDelegate;
        
        private System.Threading.SendOrPostCallback onGetAllocationsCompletedDelegate;
        
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
        
        public event System.EventHandler<GetAllocationsCompletedEventArgs> GetAllocationsCompleted;
        
        public WcfServiceLibrary.AllocationsData GetAllocations(WcfServiceLibrary.ConfigData config, int deadline) {
            return base.Channel.GetAllocations(config, deadline);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetAllocations(WcfServiceLibrary.ConfigData config, int deadline, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetAllocations(config, deadline, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public WcfServiceLibrary.AllocationsData EndGetAllocations(System.IAsyncResult result) {
            return base.Channel.EndGetAllocations(result);
        }
        
        private System.IAsyncResult OnBeginGetAllocations(object[] inValues, System.AsyncCallback callback, object asyncState) {
            WcfServiceLibrary.ConfigData config = ((WcfServiceLibrary.ConfigData)(inValues[0]));
            int deadline = ((int)(inValues[1]));
            return this.BeginGetAllocations(config, deadline, callback, asyncState);
        }
        
        private object[] OnEndGetAllocations(System.IAsyncResult result) {
            WcfServiceLibrary.AllocationsData retVal = this.EndGetAllocations(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetAllocationsCompleted(object state) {
            if ((this.GetAllocationsCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetAllocationsCompleted(this, new GetAllocationsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetAllocationsAsync(WcfServiceLibrary.ConfigData config, int deadline) {
            this.GetAllocationsAsync(config, deadline, null);
        }
        
        public void GetAllocationsAsync(WcfServiceLibrary.ConfigData config, int deadline, object userState) {
            if ((this.onBeginGetAllocationsDelegate == null)) {
                this.onBeginGetAllocationsDelegate = new BeginOperationDelegate(this.OnBeginGetAllocations);
            }
            if ((this.onEndGetAllocationsDelegate == null)) {
                this.onEndGetAllocationsDelegate = new EndOperationDelegate(this.OnEndGetAllocations);
            }
            if ((this.onGetAllocationsCompletedDelegate == null)) {
                this.onGetAllocationsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetAllocationsCompleted);
            }
            base.InvokeAsync(this.onBeginGetAllocationsDelegate, new object[] {
                        config,
                        deadline}, this.onEndGetAllocationsDelegate, this.onGetAllocationsCompletedDelegate, userState);
        }
    }
}
