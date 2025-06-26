using AmitalCloud.Infrastructure.Domain.Helpers;
using System;
using System.IO;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class ExceptionHandler
    {

        public static void HandleException(Exception exception, DateTime clientDate, int tenant, string userId, string userName, string ExtraMessage, string ip)
        {
            string ErrorMessage = "";

            try
            {


                if (!DBHelpers.DbContextBaseUtil.MaxPoolSizeWasReachedWhileSave.HasValue && exception != null && exception.ToString().Contains("max pool size was reached"))//  The timeout period elapsed prior to obtaining a connection from the pool.  This may have occurred because all pooled connections were in use and max pool size was reached.
                {
                    DBHelpers.DbContextBaseUtil.MaxPoolSizeWasReachedWhileSave = DateTime.Now;
                }

                exception = exception ?? new Exception(ExtraMessage ?? "");

                if (exception.ToString().Contains("max pool size was reached"))
                {
                    if (InjectionUtil.Instance.IISManager != null)
                    {
                        InjectionUtil.Instance.IISManager.RecycleMe();
                    }
                }

                NetCommonHelper.Logger.DevLog.Instance.WriteFatal(exception);
                

                if (!string.IsNullOrEmpty(ip) && ip.StartsWith("150.70"))
                {

                    return;

                }



                if (exception != null)
                {
                    if (exception.Message.Contains("Sorry you can't update this record right now. It's being updated by another user")
                        || exception.Message.Contains("WebFreight.Web.Security.AutenticationException")
                        || exception.Message.Contains("Sorry! this user is not the last signed user!")
                        || exception.Message.Contains("Can't create payment with future date")
                        || exception.Message.Contains("is required")
                        || exception.Message.Contains("already exists")
                        || exception.Message.Contains("Master field already used in another Shipment")
                        || exception.Message.Contains("You should have at least 1 invoice line")
                        || exception.Message.Contains("Some of invoice lines Vat Type Percentage is empty")
                        || exception.Message.Contains("Invoice line amount field must not be zero")
                        || exception.Message.Contains("You should have at least 1 invAPInvoice.M.VatTypePercentageEmptyoice line")
                    || exception.Message.Contains("Sorry! you have no permission to do this operation"))
                    {
                        return;
                    }
                    if (!string.IsNullOrEmpty(ExtraMessage))
                    {

                        ErrorMessage = ExtraMessage + Environment.NewLine;
                        ErrorMessage = ExtraMessage + Environment.NewLine;
                    }

                    ErrorMessage += exception.Message;

                    ErrorMessage += exception.Message;

                    if (exception.InnerException != null)
                    {
                        ErrorMessage += Environment.NewLine + exception.InnerException.Message;

                        if (exception.InnerException.InnerException != null)
                        {
                            ErrorMessage += Environment.NewLine + exception.InnerException.InnerException.Message;

                            if (exception.InnerException.InnerException.InnerException != null)
                            {
                                ErrorMessage += Environment.NewLine + exception.InnerException.InnerException.InnerException.Message;
                            }
                        }
                    }

                    if (clientDate == null)
                        clientDate = DateTime.Now;

                    string stacktrace = "";
                    if (exception.StackTrace != null)
                        stacktrace = exception.StackTrace;
                    NetCommonHelper.Logger.DevLog.Instance.WriteDebug("***HandleException** " + ErrorMessage);//May cause slowness ,But worth - If u Decides to delete ,Please inform itzik !!!!!
                    AzureLog.SaveLogsInStorage(ErrorMessage, "E", clientDate, exception.Message, exception.StackTrace, tenant, userId, userName, ip, exception);

                }

            }
            catch (Exception eee)
            {

                OnExceptionOnDbLogInFile(eee);
            }
        }

        private static void OnExceptionOnDbLogInFile(Exception eee)
        {
            try
            {
                string fileN = Path.Combine(Path.GetTempPath(), "AmitalCustomsError.Log");
                File.WriteAllText(fileN, eee.ToString());
            }
            catch (Exception)
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug("Unable to write to File (OnExceptionOnDbLogInFile)");
                //throw;
            }
        }

        public static void HandleDbException(Exception exception, string TypeOrUser, string ExtraMessage)
        {
            HandleException(exception, DateTime.Now, 0, TypeOrUser, "", ExtraMessage, "");
        }
    }
}