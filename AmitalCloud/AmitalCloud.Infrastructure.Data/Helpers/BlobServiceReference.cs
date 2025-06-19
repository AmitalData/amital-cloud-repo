
namespace AmitalCloud.Infrastructure.Data.BlobServiceReference
{
    using System.Runtime.Serialization;
    using System;
    using System.ServiceModel;

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "Response", Namespace = "http://schemas.datacontract.org/2004/07/AmitalCloud.FilingStorageService")]
    [System.SerializableAttribute()]
    public partial class Response : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {

        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorMessageField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool HasErrorField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string InnerErrorMessageField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private object ResultField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorMessage
        {
            get
            {
                return this.ErrorMessageField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ErrorMessageField, value) != true))
                {
                    this.ErrorMessageField = value;
                    this.RaisePropertyChanged("ErrorMessage");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool HasError
        {
            get
            {
                return this.HasErrorField;
            }
            set
            {
                if ((this.HasErrorField.Equals(value) != true))
                {
                    this.HasErrorField = value;
                    this.RaisePropertyChanged("HasError");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string InnerErrorMessage
        {
            get
            {
                return this.InnerErrorMessageField;
            }
            set
            {
                if ((object.ReferenceEquals(this.InnerErrorMessageField, value) != true))
                {
                    this.InnerErrorMessageField = value;
                    this.RaisePropertyChanged("InnerErrorMessage");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public object Result
        {
            get
            {
                return this.ResultField;
            }
            set
            {
                if ((object.ReferenceEquals(this.ResultField, value) != true))
                {
                    this.ResultField = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName = "BlobServiceReference.IBlobService")]
    public interface IBlobService
    {

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/DoWork", ReplyAction = "http://tempuri.org/IBlobService/DoWorkResponse")]
        void DoWork();

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/DoWork", ReplyAction = "http://tempuri.org/IBlobService/DoWorkResponse")]
        System.Threading.Tasks.Task DoWorkAsync();

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/Read", ReplyAction = "http://tempuri.org/IBlobService/ReadResponse")]
        AmitalCloud.Infrastructure.Data.BlobServiceReference.Response Read(string filePath);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/Read", ReplyAction = "http://tempuri.org/IBlobService/ReadResponse")]
        System.Threading.Tasks.Task<AmitalCloud.Infrastructure.Data.BlobServiceReference.Response> ReadAsync(string filePath);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/Write", ReplyAction = "http://tempuri.org/IBlobService/WriteResponse")]
        AmitalCloud.Infrastructure.Data.BlobServiceReference.Response Write(byte[] data, string filePath);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/Write", ReplyAction = "http://tempuri.org/IBlobService/WriteResponse")]
        System.Threading.Tasks.Task<AmitalCloud.Infrastructure.Data.BlobServiceReference.Response> WriteAsync(byte[] data, string filePath);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/WriteBlock", ReplyAction = "http://tempuri.org/IBlobService/WriteBlockResponse")]
        AmitalCloud.Infrastructure.Data.BlobServiceReference.Response WriteBlock(byte[] buffer, long fileSize, long sentBytes, string filePath, string fileName);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/WriteBlock", ReplyAction = "http://tempuri.org/IBlobService/WriteBlockResponse")]
        System.Threading.Tasks.Task<AmitalCloud.Infrastructure.Data.BlobServiceReference.Response> WriteBlockAsync(byte[] buffer, long fileSize, long sentBytes, string filePath, string fileName);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/Delete", ReplyAction = "http://tempuri.org/IBlobService/DeleteResponse")]
        AmitalCloud.Infrastructure.Data.BlobServiceReference.Response Delete(string filePath);

        [System.ServiceModel.OperationContractAttribute(Action = "http://tempuri.org/IBlobService/Delete", ReplyAction = "http://tempuri.org/IBlobService/DeleteResponse")]
        System.Threading.Tasks.Task<AmitalCloud.Infrastructure.Data.BlobServiceReference.Response> DeleteAsync(string filePath);
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBlobServiceChannel : AmitalCloud.Infrastructure.Data.BlobServiceReference.IBlobService, System.ServiceModel.IClientChannel
    {
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BlobServiceClient : System.ServiceModel.ClientBase<AmitalCloud.Infrastructure.Data.BlobServiceReference.IBlobService>, AmitalCloud.Infrastructure.Data.BlobServiceReference.IBlobService
    {

        public BlobServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public void DoWork()
        {
            base.Channel.DoWork();
        }

        public System.Threading.Tasks.Task DoWorkAsync()
        {
            return base.Channel.DoWorkAsync();
        }

        public AmitalCloud.Infrastructure.Data.BlobServiceReference.Response Read(string filePath)
        {
            return base.Channel.Read(filePath);
        }

        public System.Threading.Tasks.Task<AmitalCloud.Infrastructure.Data.BlobServiceReference.Response> ReadAsync(string filePath)
        {
            return base.Channel.ReadAsync(filePath);
        }

        public AmitalCloud.Infrastructure.Data.BlobServiceReference.Response Write(byte[] data, string filePath)
        {
            return base.Channel.Write(data, filePath);
        }

        public System.Threading.Tasks.Task<AmitalCloud.Infrastructure.Data.BlobServiceReference.Response> WriteAsync(byte[] data, string filePath)
        {
            return base.Channel.WriteAsync(data, filePath);
        }

        public AmitalCloud.Infrastructure.Data.BlobServiceReference.Response WriteBlock(byte[] buffer, long fileSize, long sentBytes, string filePath, string fileName)
        {
            return base.Channel.WriteBlock(buffer, fileSize, sentBytes, filePath, fileName);
        }

        public System.Threading.Tasks.Task<AmitalCloud.Infrastructure.Data.BlobServiceReference.Response> WriteBlockAsync(byte[] buffer, long fileSize, long sentBytes, string filePath, string fileName)
        {
            return base.Channel.WriteBlockAsync(buffer, fileSize, sentBytes, filePath, fileName);
        }

        public AmitalCloud.Infrastructure.Data.BlobServiceReference.Response Delete(string filePath)
        {
            return base.Channel.Delete(filePath);
        }

        public System.Threading.Tasks.Task<AmitalCloud.Infrastructure.Data.BlobServiceReference.Response> DeleteAsync(string filePath)
        {
            return base.Channel.DeleteAsync(filePath);
        }
    }
}
