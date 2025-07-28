using System.Linq;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using System.Transactions;
using System;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ErrorLogQuery
    {
        private readonly Repository<ErrorLog> repository;
        private readonly int tenant;

        public ErrorLogQuery(int tenant)
        {
            repository = new Repository<ErrorLog>(SystemLogContext.GetContext(tenant));
        }

        //public ErrorLog AddErrorLog(ErrorLog errorLog)
        //{
        //    using (TransactionScope scope = TransactionFactory.GetTransaction())
        //    {
        //        ISystemLogContext systemLogContext = SystemLogContext.GetContext(tenant);

        //        if (errorLog.Id == null)
        //        {
        //            errorLog.Id = Guid.NewGuid().ToString();
        //        }

        //        try
        //        {
        //            if (!repository.GetMulti(a => a.Id == errorLog.Id).Any())
        //            {
        //                errorLog.SearchFields = errorLog.Tier + "," + errorLog.UserName + "," + errorLog.Exception + "," + errorLog.Tenant + "," + errorLog.IP;

        //                if (!string.IsNullOrEmpty(errorLog.Exception) && errorLog.Exception.Length > 7000)
        //                {
        //                    errorLog.Exception = errorLog.Exception.Substring(0, 7000);
        //                }
        //                if (!string.IsNullOrEmpty(errorLog.StackTrace) && errorLog.StackTrace.Length > 7000)
        //                {
        //                    errorLog.StackTrace = errorLog.StackTrace.Substring(0, 7000);
        //                }
        //                if (!string.IsNullOrEmpty(errorLog.SearchFields) && errorLog.SearchFields.Length > 8000)
        //                {
        //                    errorLog.SearchFields = errorLog.SearchFields.Substring(0, 8000);
        //                }

        //                repository.Insert(errorLog);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            if (ex.InnerException != null)
        //            {
        //                if (ex.InnerException.Message.Contains("Violation of PRIMARY KEY constraint") || ex.Message.Contains("Violation of PRIMARY KEY constraint"))
        //                {
        //                    try
        //                    {
        //                        errorLog.Id = Guid.NewGuid().ToString();
        //                        repository.SubmitChanges();
        //                    }
        //                    catch (Exception)
        //                    {
        //                        NetCommonHelper.Logger.DevLog.Instance.WriteWarning($"Failed to insert an error log: {ex.Message}");
        //                    }
        //                }
        //            }
        //        }
        //        scope.Complete();
        //    }
        //    return errorLog;
        //}
    }
}