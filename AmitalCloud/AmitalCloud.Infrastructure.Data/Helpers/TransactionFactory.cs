using AmitalCloud.Infrastructure.Domain.DataContracts;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Transactions;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class TransactionFactory
    {

        public static TransactionScope GetNewTransactionSuppress()// testing 1 2 3  ???  
        {
            //#if ORACLE_DB

            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                return new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted });
            }

            //#endif

            return new TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = IsolationLevel.Snapshot });

        }

        public static TransactionScope GetNewTransaction(TimeSpan? timeOut = null)
        {
            //#if ORACLE_DB

            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                if (timeOut != null)
                {
                    return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { Timeout = timeOut.Value, IsolationLevel = IsolationLevel.ReadCommitted });
                }
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted });
            }

            //#endif
            if (timeOut != null)
            {
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.Snapshot, Timeout = timeOut.Value });
            }
            return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.Snapshot });

        }
        public static TransactionScope GetNewOracleReadCommittedTransaction(TimeSpan? timeOut = null)
        {
#if ORACLE_DB
            if (timeOut != null)
            {
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { Timeout = timeOut.Value /*IsolationLevel = IsolationLevel.Snapshot*/ });
            }
            return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() {/*IsolationLevel = IsolationLevel.Snapshot*/ });
#endif
            if (timeOut == null)
            {
                timeOut = TimeSpan.FromMinutes(3);
            }
            if (timeOut != null)
            {
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = timeOut.Value });
            }
            return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted });

        }
        public static TransactionScope GetNewSerializableTransaction(TimeSpan? timeOut = null)
        {
            //#if ORACLE_DB
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                if (timeOut != null)
                {
                    return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { Timeout = timeOut.Value, IsolationLevel = IsolationLevel.Serializable });
                }
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.Serializable });
            }
            //#endif
            if (timeOut != null)
            {
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.Serializable, Timeout = timeOut.Value });
            }
            return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.Serializable });

        }

        public static TransactionScope GetTransaction(TimeSpan? timeOut = null)
        {
            //#if ORACLE_DB
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                if (timeOut != null)
                {
                    if (Transaction.Current != null)
                    {
                        return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { Timeout = timeOut.Value, IsolationLevel = IsolationLevel.ReadCommitted });
                    }
                    return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { Timeout = timeOut.Value, IsolationLevel = IsolationLevel.ReadCommitted });
                }
                if (Transaction.Current != null)
                {
                    return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted });
                }
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted });
            }
            //#endif
            if (timeOut != null)
            {
                if (Transaction.Current != null)
                {
                    return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.Snapshot, Timeout = timeOut.Value });
                }
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.Snapshot, Timeout = timeOut.Value });
            }
            if (Transaction.Current != null)
            {
                return new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = Transaction.Current.IsolationLevel });
            }
            return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.Snapshot });
        }

        private static void SetTransactionManagerField(string fieldName, object value)
        {
            typeof(TransactionManager).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value);
        }

        public static TransactionScope CreateTransactionScope(TimeSpan timeout)
        {
            // or for netcore / .net5+ use these names instead:
            //    s_cachedMaxTimeout
            //    s_maximumTimeout
            SetTransactionManagerField("_cachedMaxTimeout", true);
            SetTransactionManagerField("_maximumTimeout", timeout);
            return new TransactionScope(TransactionScopeOption.RequiresNew, timeout);
        }

        public static TransactionScope GetNewTransactionWithDefaultIsolationLevel(TimeSpan? timeOut = null)
        {
            if (timeOut != null)
            {
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { Timeout = timeOut.Value });
            }
            return new TransactionScope(TransactionScopeOption.RequiresNew);
        }

        public static TransactionScope GetNewReadCommittedTransaction(TimeSpan? timeOut = null)
        {
            if (timeOut != null)
            {
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = timeOut.Value });
            }
            return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted });
        }

        public static TransactionScope GetNewReadUncommittedTransaction(TimeSpan? timeOut = null)
        {
            if (timeOut != null)
            {
                return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted, Timeout = timeOut.Value });
            }
            return new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadUncommitted });
        }
        public static void RegisterTransactionCompleted()
        {
            if (Transaction.Current == null) return;
            //Register for the transaction completed event for the current transaction
            Transaction.Current.TransactionCompleted -= new TransactionCompletedEventHandler(Current_TransactionCompleted);
            Transaction.Current.TransactionCompleted += new TransactionCompletedEventHandler(Current_TransactionCompleted);


        }
        static void Current_TransactionCompleted(object sender, TransactionEventArgs e)
        {
            NetCommonHelper.Logger.DevLog.Instance.WriteDebug(Environment.StackTrace.ToString());
            NetCommonHelper.Logger.DevLog.Instance.WriteDebug("A transaction has completed:" + JsonConvert.SerializeObject(e.Transaction));
        }
    }


    public class TransactionFactoryWrapper
    {
        public virtual IDisposable GetTransaction(TimeSpan? timeOut = null)
        {
            return TransactionFactory.GetTransaction(timeOut) as IDisposable;
        }
    }

}
