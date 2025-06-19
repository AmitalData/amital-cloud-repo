using AmitalCloud.Infrastructure.Application.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Transactions;
using System.Web;

namespace AmitalCloud.Infrastructure.Application.BaseClasses
{
    public abstract class AmitalCloudDomainService  
    {
   

        private void HandleExceptionOnUpdate(DbUpdateException e)
        {
            var errorMessages = new StringBuilder();

            var entries = e.Entries;
            foreach (var entry in entries)
            {
                errorMessages.AppendLine($"Entity of type {entry.Entity.GetType().Name} in state {entry.State} caused an error.");
            }

            if (e.InnerException != null)
            {
                errorMessages.AppendLine("Inner exception: " + e.InnerException.Message);
                if (e.InnerException.InnerException != null)
                {
                    errorMessages.AppendLine("Inner-inner exception: " + e.InnerException.InnerException.Message);
                }
            }

            string Error = errorMessages.ToString();

            var (authenticateduser, ip) = AmitalCloudSecurityUtility.GetAuditInfo();

            ExceptionHandler.HandleException(new Exception(Error), DateTime.Now, 0, "", authenticateduser, "", ip);
        }
    }
}