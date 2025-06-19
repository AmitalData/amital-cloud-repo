using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class ProcessLockTableUtil
    {
        private ProcessLockTableUtil()
        {

        }

        static ProcessLockTableUtil _Instance;
        public static ProcessLockTableUtil Instance
        {
            get
            {
                ProcessLockTableUtil._Instance = ProcessLockTableUtil._Instance ?? new ProcessLockTableUtil();

                return ProcessLockTableUtil._Instance;
            }

        }

        List<ProccesLockData> _TheLockKeys = new List<ProccesLockData>();


        IDisposable LockItAndGetReleaseToken(string key2insert, string requestLog)
        {
            ProcessLockReleaseToken processLockToken = null;
            lock ((this._TheLockKeys as ICollection).SyncRoot)
            {
                var lockRow = _TheLockKeys.FirstOrDefault(r => r.MyKey == key2insert);
                if (lockRow != null)
                {
                    var mess = "ProcessLockUtil:LockItAndGetReleaseToken:fAILED:Already EXIST:" + lockRow.ToString();
                    LogMessagingUtil.Instance.AppendLine(mess);
                    throw new ProcessLockException(mess);
                }

                var newLock = new ProccesLockData()
                {
                    MyKey = key2insert,
                    MyLog = requestLog,
                    InsertTime = DateTime.Now

                };
                processLockToken = new ProcessLockReleaseToken()
                {
                    ProccesLockData = newLock
                };
                _TheLockKeys.Add(newLock);
                LogMessagingUtil.Instance.AppendLine("ProcessLockUtil: LockItAndGetReleaseToken:ADD<<<" + newLock.ToString());

            }
            return processLockToken as IDisposable;
        }
        private void RealseKey(ProcessLockReleaseToken disposeProcessLockToken)
        {
            lock ((this._TheLockKeys as ICollection).SyncRoot)
            {
                var lockRow = _TheLockKeys.FirstOrDefault(r => r.MyKey == disposeProcessLockToken.ProccesLockData.MyKey);
                if (lockRow == null)
                {

                    throw new ProcessLockException("ProcessLockUtil: RealseKey:FAILED:Already removed !!!!" + disposeProcessLockToken.ProccesLockData.ToString());
                }


                _TheLockKeys.Remove(lockRow);
                LogMessagingUtil.Instance.AppendLine("ProcessLockUtil:RealseKey:Removed>>>:" + lockRow.ToString());

            }
        }
        public string GetKey4DocumentsFilingId(string DocumentsFilingId, int tenant)
        {
            string key = "DocFilingId:" + DocumentsFilingId + ",t:" +
                       tenant.ToString();
            return key;

        }
        public string GetKey4UpdateDeclarationCourier_DocumentStatusCode(string DecId, int tenant)
        {
            string key = "CourierDocStatus:DecId:" + DecId + ",T:" + tenant.ToString();
            return key;

        }
        public string GetKey4Declaration(string declarationNumber, int tenant)
        {
            string key = "ResponseService,declarationNumber:" + declarationNumber + ",t:" +
                       tenant.ToString();
            return key;
        }
        public string GetKey4UCBUD2LT(string DocumentsFilingId, int tenant)
        {
            string key = "UCBUD2LT:DocFilingId:" + DocumentsFilingId + ",t:" +
                       tenant.ToString();
            return key;
        }
        public string GetKey4InProggressCustomsRequestsSheet(string CustomsRequestsSheetId)
        {
            string key = "InProggressCustomsRequestsSheet:" + CustomsRequestsSheetId;
            return key;
        }

        public class ProcessLockReleaseToken : IDisposable
        {
            public ProccesLockData ProccesLockData { get; internal set; }

            public void Dispose()
            {
                ProcessLockTableUtil.Instance.RealseKey(this);
            }
        }

        public IDisposable GetProcessLockTableDisposable(int tenant, bool lockit, string key, string requestLog, bool? forceAsMultiProcess = null)
        {
            if (lockit)
            {
                if (forceAsMultiProcess ?? !string.IsNullOrWhiteSpace(ConfigurationHelper.GetConnectionString("MultiProcess")))
                {
                    return new MultiProcessLockTableUtil().LockItAndGetReleaseToken(tenant, key, requestLog);
                }
                return Instance.LockItAndGetReleaseToken(key, requestLog);
            }
            return new LockTableDisposable();
        }
    }
    public class ProcessLockException : Exception
    {
        public ProcessLockException(string LogMessage)
        {

        }
    }

    public class ProccesLockData
    {
        public int Tenant { get; set; }
        public string MyKey { get; set; }
        public string MyLog { get; set; }
        public DateTime InsertTime { get; set; }
        public override string ToString()
        {
            return $"Tenant{Tenant},mykey:{this.MyKey},mylog:{this.MyLog},InsertAt:{this.InsertTime}";
        }
    }
    public class LockTableDisposable : IDisposable
    {


        public void Dispose()
        {

        }
    }

}
