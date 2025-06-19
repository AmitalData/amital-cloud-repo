using System;
using System.Data.Common;
using System.Data.SqlClient;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class DatabaseInitializer
    {
        [ThreadStatic] public static bool RunOnSeconderyDB = false;
        public static DbConnection GetConnection(string dbConnectionInfo, string SeconderyDBConnectionInfo)//SeconderyDBConnectionInfo = null
        {
            if (RunOnSeconderyDB == true && !string.IsNullOrEmpty(SeconderyDBConnectionInfo))
            {
                return GetConnection(SeconderyDBConnectionInfo, null, null);
            }
            else
            {
                return GetConnection(dbConnectionInfo, null, null);
            }
        }
        public static string GetConnectionString(string dbConnectionInfo, string SeconderyDBConnectionInfo)//SeconderyDBConnectionInfo = null
        {
            if (RunOnSeconderyDB == true && !string.IsNullOrEmpty(SeconderyDBConnectionInfo))
            {
                return GetConnectionString(SeconderyDBConnectionInfo, null, null);
            }
            else
            {
                return GetConnectionString(dbConnectionInfo, null, null);
            }
        }
        public static string GetConnectionString(string dbConnectionInfo, int? connectionLifetime = null, bool? suppressPool = null)
        {
                return GetSQLServerConnectionString(dbConnectionInfo, connectionLifetime);
        }
        public static DbConnection GetConnection(string dbConnectionInfo, int? connectionLifetime = null, bool? suppressPool = null)
        {
                return GetSQLServerConnecttion(dbConnectionInfo, connectionLifetime);
        }

        private static DbConnection GetSQLServerConnecttion(string dbConnectionInfo, int? connectionLifetime = null)
        {
            string providerString = GetSQLServerConnectionString(dbConnectionInfo, connectionLifetime);
            return new SqlConnection(providerString);
        }

        private static string GetSQLServerConnectionString(string dbConnectionInfo, int? connectionLifetime = null)
        {
            string[] information = dbConnectionInfo.Split(',');
            string databaseName = information[0];
            string userName = information[1];
            string pass = information[2];
            string servername = information[3].Replace(":", ",");
            string applicationIntent = information.Length > 4 ? information[4] : "";
            SqlConnectionStringBuilder sqlBuilder =
                new SqlConnectionStringBuilder();
            sqlBuilder.DataSource = servername;
            sqlBuilder.InitialCatalog = databaseName;
            sqlBuilder.PersistSecurityInfo = true;
            sqlBuilder.IntegratedSecurity = false;
            sqlBuilder.Password = pass;
            sqlBuilder.UserID = userName;
            sqlBuilder.MultipleActiveResultSets = true;
            if (!string.IsNullOrEmpty(applicationIntent))
            {
                sqlBuilder.ApplicationIntent = applicationIntent == "ReadOnly" ? ApplicationIntent.ReadOnly : ApplicationIntent.ReadWrite;
            }
            if (connectionLifetime.HasValue && connectionLifetime.Value > 0)
            {
                sqlBuilder.ConnectTimeout = connectionLifetime.Value;
            }
            else
            {
                sqlBuilder.ConnectTimeout = 100; // old value was 60 . the 100 added by Rabaia with mohammad in order to fix Time Out Problem caused by Lock on get next counter "the update is temporary"
            }
            sqlBuilder.MaxPoolSize = 200;
            string providerString = sqlBuilder.ToString();
            return providerString;
        }
    }
}
