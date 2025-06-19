using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;
using System.Configuration;
using System.Globalization;
using System.Text;
using System.Transactions;
namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public class ChangesLoggerService
    {
        public void LogIt<PMType, POCOType>(
            string appSettingKeyValueIsLogUntilDateyyyyMMdd,
            PMType entityPM, POCOType entityPOCO)
        {

            try
            {
                if (!IsLogEnable(appSettingKeyValueIsLogUntilDateyyyyMMdd))
                {
                    return;
                }


                string jsonPM = ProxyHelper.JsonConvertSerialize(entityPM);
                string jsonPOCO = ProxyHelper.JsonConvertSerialize(entityPOCO);
                string resolveUserIdentityName = GetUserIdentityName();
                string trans = GetTransaction();
                var sb = new StringBuilder();
                sb
                    .AppendLine("Transaction.Current:")
                    .AppendLine(trans)

                    .AppendLine("ResolveUserIdentityName:")
                    .AppendLine(resolveUserIdentityName)
                    .AppendLine("**Stack:")
                    .AppendLine(Environment.StackTrace)
                    .AppendLine("**PM:New:")
                    .AppendLine(jsonPM)
                    .AppendLine("**POCO:old:")
                    .AppendLine(jsonPOCO);

                AmitalCloudSettings.HandleLogMe
                    //(mess, err, suffix, stopLogAt)
                    (sb.ToString(), false, appSettingKeyValueIsLogUntilDateyyyyMMdd, DateTime.MaxValue);
            }
            catch (Exception eeex)
            {


            }

        }

        public void SBLog(
            string appSettingKeyValueIsLogUntilDateyyyyMMdd,
            StringBuilder stringBuilder
            )
        {
            try
            {
                if (!IsLogEnable(appSettingKeyValueIsLogUntilDateyyyyMMdd))
                {
                    return;
                }
                string resolveUserIdentityName = GetUserIdentityName();
                string trans = GetTransaction();
                stringBuilder
                    .AppendLine("Transaction.Current:")
                    .AppendLine(trans)

                    .AppendLine("ResolveUserIdentityName:")
                    .AppendLine(resolveUserIdentityName)
                    .AppendLine("**Stack:")
                    .AppendLine(Environment.StackTrace)
                    .AppendLine();

                AmitalCloudSettings.HandleLogMe
                    (stringBuilder.ToString(), false, appSettingKeyValueIsLogUntilDateyyyyMMdd, DateTime.MaxValue);

            }
            catch (Exception)
            {

                //throw;
            }
        }

        private static string GetTransaction()
        {
            string trans = "-999";
            if (Transaction.Current != null)
            {
                trans = Transaction.Current.GetHashCode().ToString();
            }

            return trans;
        }

        private static string GetUserIdentityName()
        {
            var resolveUserIdentityName = "";
            try
            {
                resolveUserIdentityName = AuthenticationUtil.ResolveUserIdentityName(0);
            }
            catch
            {

            }

            return resolveUserIdentityName;
        }


        public static bool IsLogEnable(string appSettingKeyValueIsLogUntilDateyyyyMMdd)
        {
            try
            {
                string UntilDateyyyyMMdd = ConfigurationHelper.GetValue(appSettingKeyValueIsLogUntilDateyyyyMMdd);
                if (string.IsNullOrWhiteSpace(UntilDateyyyyMMdd))
                {
                    return false;
                }

                DateTime stopLogAt = DateTime.MinValue; //new DateTime(2018, 02, 20);
                stopLogAt = DateTime.ParseExact(UntilDateyyyyMMdd,
                                                        "yyyyMMdd",
                                                        CultureInfo.InvariantCulture,
                                                        DateTimeStyles.None);

                return !(DateTime.Now > stopLogAt);
            }
            catch
            {
                return false;
            }
        }
    }
}
