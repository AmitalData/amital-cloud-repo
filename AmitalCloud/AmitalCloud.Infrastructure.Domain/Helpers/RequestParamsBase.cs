using System;
using System.ComponentModel;

namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public enum SendRequestVIA
    {
        Default,// from CustomsMessaging Library
        WebServiceInteractive,
        WebServiceBatch,
        DCABatch
    }
    public abstract class RequestParamsBase : INotifyPropertyChanged
    {


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
        public RequestParamsBase()
        {
            this.PBId = Guid.NewGuid().ToString();
        }

        public bool LoggingEnabled { get; set; }

        public string LoggingEntityReference { get; set; }
        public string LoggingObjectTableId { get; set; }
        public string LoggingEntityId { get; set; }
        public string LoggingObjectTableId2 { get; set; }
        public string LoggingEntityId2 { get; set; }

        public string LoggingUserId { get; set; }
        public bool IsFakeResponse { get; set; }
        public int Tenant { get; set; }
        public int TenantPriority { get; set; }
        public string RequestName { get; set; }
        public string ResponseName { get; set; }
        public TestCase TestCase { get; set; }

        //public SendRequestVIA UserRequestVIA { get; set; }
        public string RequestVIAChangeDue { get; set; }
        /// Itzik Add
        public SendRequestVIA RequestVIA { get; set; }
        //public SendRequestVIA RealVIA { get; set; } //NO DEFAULT 
        //public string DCAFileName { get; set; }
        public string DCAFileName { get; set; } // itzik revive it 
        public string InterfaceTypeCode { get; set; }
        public string MainInterfaceCode { get; set; }
        public string CustomsRequestsSheetId { get; set; }

        public DateTime? TransmitionDateTime { get; set; } //4 dca in  - set CRS Create Date By DCA TransmitionDateTime
        public DateTime? FutureSendDateTime { get; set; }// 4 2755 by  FuturePaymenDateTime Build Queue 4 future 
        ///public bool StartInteractive { get; set; }
        /// <summary>
        //the eblity to continue work 1 proccess without Split WorkerRole
        // if 1 WorkerRoleName  =SBQueueNames.CustomsMessagingSheetBQ
        //Else WorkerRoleName Stats with CustomsCommand*********WR 
        //and end with WR
        //to Insure Look At Log 
        //currentWR =CustomsCommandGetCustomRequestWR 
        //currentWR =CustomsCommandSendWSReceiveCorrelationWR 
        /// </summary>
        public bool SuppressSplitWR { get; set; }

        public string PBId { get; set; }

        public bool ForcePersonalSign { get; set; }

        public bool IsAngularClient { get; set; }

        public bool DeleteStatus { get; set; }


        public string UnifreightListOnServerOnly { get; set; }

        public bool SplitterModeLetCreateMyType { get; set; }

        public bool AvoidSign { get; set; }


        public string SignMethodByQueue { get; set; }
        public string SignByPersonalId { get; set; }
        public string SignQueueByCompanyOrPersonal { get; set; }




        public string ParentId { get; set; }


        public bool ForceCompanySign { get; set; }



    }

    public class TestCase
    {
        public string Code { get; set; }
        public string Param1 { get; set; }
        public string Param2 { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string Group { get; set; }
        public int IndexOrder { get; set; }

    }
}
