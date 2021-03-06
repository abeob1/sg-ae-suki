﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.8009
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

// 
// This source code was auto-generated by wsdl, Version=2.0.50727.3038.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Web.Services.WebServiceBindingAttribute(Name="GetDefaultSoap", Namespace="http://tempuri.org/")]
public partial class GetDefault : System.Web.Services.Protocols.SoapHttpClientProtocol {
    
    private System.Threading.SendOrPostCallback GetCopyFromToOperationCompleted;
    
    private System.Threading.SendOrPostCallback LogOutOperationCompleted;
    
    private System.Threading.SendOrPostCallback TestConnectionOperationCompleted;
    
    /// <remarks/>
    public GetDefault() {
        this.Url = "http://localhost:49310/GetDefault.asmx";
    }
    
    /// <remarks/>
    public event GetCopyFromToCompletedEventHandler GetCopyFromToCompleted;
    
    /// <remarks/>
    public event LogOutCompletedEventHandler LogOutCompleted;
    
    /// <remarks/>
    public event TestConnectionCompletedEventHandler TestConnectionCompleted;
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetCopyFromTo", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public System.Data.DataSet GetCopyFromTo(int Type, string ObjType) {
        object[] results = this.Invoke("GetCopyFromTo", new object[] {
                    Type,
                    ObjType});
        return ((System.Data.DataSet)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginGetCopyFromTo(int Type, string ObjType, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("GetCopyFromTo", new object[] {
                    Type,
                    ObjType}, callback, asyncState);
    }
    
    /// <remarks/>
    public System.Data.DataSet EndGetCopyFromTo(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((System.Data.DataSet)(results[0]));
    }
    
    /// <remarks/>
    public void GetCopyFromToAsync(int Type, string ObjType) {
        this.GetCopyFromToAsync(Type, ObjType, null);
    }
    
    /// <remarks/>
    public void GetCopyFromToAsync(int Type, string ObjType, object userState) {
        if ((this.GetCopyFromToOperationCompleted == null)) {
            this.GetCopyFromToOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetCopyFromToOperationCompleted);
        }
        this.InvokeAsync("GetCopyFromTo", new object[] {
                    Type,
                    ObjType}, this.GetCopyFromToOperationCompleted, userState);
    }
    
    private void OnGetCopyFromToOperationCompleted(object arg) {
        if ((this.GetCopyFromToCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.GetCopyFromToCompleted(this, new GetCopyFromToCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/LogOut", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public int LogOut() {
        object[] results = this.Invoke("LogOut", new object[0]);
        return ((int)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginLogOut(System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("LogOut", new object[0], callback, asyncState);
    }
    
    /// <remarks/>
    public int EndLogOut(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((int)(results[0]));
    }
    
    /// <remarks/>
    public void LogOutAsync() {
        this.LogOutAsync(null);
    }
    
    /// <remarks/>
    public void LogOutAsync(object userState) {
        if ((this.LogOutOperationCompleted == null)) {
            this.LogOutOperationCompleted = new System.Threading.SendOrPostCallback(this.OnLogOutOperationCompleted);
        }
        this.InvokeAsync("LogOut", new object[0], this.LogOutOperationCompleted, userState);
    }
    
    private void OnLogOutOperationCompleted(object arg) {
        if ((this.LogOutCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.LogOutCompleted(this, new LogOutCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/TestConnection", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
    public string TestConnection(string ConnectionString) {
        object[] results = this.Invoke("TestConnection", new object[] {
                    ConnectionString});
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public System.IAsyncResult BeginTestConnection(string ConnectionString, System.AsyncCallback callback, object asyncState) {
        return this.BeginInvoke("TestConnection", new object[] {
                    ConnectionString}, callback, asyncState);
    }
    
    /// <remarks/>
    public string EndTestConnection(System.IAsyncResult asyncResult) {
        object[] results = this.EndInvoke(asyncResult);
        return ((string)(results[0]));
    }
    
    /// <remarks/>
    public void TestConnectionAsync(string ConnectionString) {
        this.TestConnectionAsync(ConnectionString, null);
    }
    
    /// <remarks/>
    public void TestConnectionAsync(string ConnectionString, object userState) {
        if ((this.TestConnectionOperationCompleted == null)) {
            this.TestConnectionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnTestConnectionOperationCompleted);
        }
        this.InvokeAsync("TestConnection", new object[] {
                    ConnectionString}, this.TestConnectionOperationCompleted, userState);
    }
    
    private void OnTestConnectionOperationCompleted(object arg) {
        if ((this.TestConnectionCompleted != null)) {
            System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
            this.TestConnectionCompleted(this, new TestConnectionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
        }
    }
    
    /// <remarks/>
    public new void CancelAsync(object userState) {
        base.CancelAsync(userState);
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
public delegate void GetCopyFromToCompletedEventHandler(object sender, GetCopyFromToCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class GetCopyFromToCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal GetCopyFromToCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public System.Data.DataSet Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((System.Data.DataSet)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
public delegate void LogOutCompletedEventHandler(object sender, LogOutCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class LogOutCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal LogOutCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public int Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((int)(this.results[0]));
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
public delegate void TestConnectionCompletedEventHandler(object sender, TestConnectionCompletedEventArgs e);

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "2.0.50727.3038")]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
public partial class TestConnectionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
    
    private object[] results;
    
    internal TestConnectionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
            base(exception, cancelled, userState) {
        this.results = results;
    }
    
    /// <remarks/>
    public string Result {
        get {
            this.RaiseExceptionIfNecessary();
            return ((string)(this.results[0]));
        }
    }
}
