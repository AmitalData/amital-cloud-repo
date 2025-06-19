using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.Enums;
using Oracle.ManagedDataAccess.Client;

namespace AmitalCloud.Infrastructure.Data.DBHelpers
{
    public static class DbContextBaseUtil
    {
        readonly static bool _UnifreightDataIncludedInMain_FeatureOn;
        public static bool UnifreightDataIncludedInMain_FeatureOn { get { return _UnifreightDataIncludedInMain_FeatureOn; } }
        static DbContextBaseUtil()
        {
            _UnifreightDataIncludedInMain_FeatureOn = true;
            return;
        }
        public static string GetConnectionStringWithAmitalNetRole(string currentConnectionString)
        {
            return currentConnectionString;
        }

        // oracle
        private static string Oracle_Globalstr;
        private static string Oracle_SystemLogsStr;

        // sql
        private static string Globalstr;
        private static string SystemLogsStr;

        public static string GlobalConnectionString
        {
            get
            {
                return AmitalCloudSettings.DatabaseManagementSystem == "oracle" ? nameof(Oracle_Globalstr) : nameof(Globalstr);
            }
        }
        public static string SystemLogsConnectionString
        {
            get
            {
                return AmitalCloudSettings.DatabaseManagementSystem == "oracle" ? nameof(Oracle_SystemLogsStr) : nameof(SystemLogsStr);
            }
        }

        public static bool? ToLog { get; set; }
        public static DateTime? MaxPoolSizeWasReachedWhileSave { get; set; }
        public static string GetStoredProcedureName(string StoredProcedure, AmitalCloudDBSchema amitalCloudDBSchema, string OracleConnectionStr)
        {
            if (UnifreightDataIncludedInMain_FeatureOn)
            {
                var oraCSB = new OracleConnectionStringBuilder(OracleConnectionStr);
                return oraCSB.UserID.ToString() + "." + StoredProcedure;
            }
            return StoredProcedure;
        }
        public static OracleConnectionStringBuilder GetOracleConStrBuilder(string dbConnectionInfo)
        {
            if (String.IsNullOrWhiteSpace(dbConnectionInfo))
            {
                throw new Exception("DevartAmitalDirect.ConnectionString is missing 'server,port,sid,userId,password'");
            }
            string[] information = dbConnectionInfo.Split(',');
            if (information.Count() != 5)
            {
                throw new Exception("DevartAmitalDirect.ConnectionString must be 'server,port,sid,userId,password'");
            }
            string server = information[0];
            string port = information[1];
            string sid = information[2];
            string userId = information[3];
            string password = information[4];
            int iPort = 1521;
            if (!int.TryParse(port, out iPort))
            {
                throw new Exception("DevartAmitalDirect.ConnectionString must be 'server,port,sid,userId,password'");
            }

            var oraCSB = new OracleConnectionStringBuilder
            {
                DataSource = $"{server}:{iPort}/{sid}",
                UserID = userId,
                Password = password
            };

            string a = oraCSB.UserID;

            return oraCSB;
        }

        public static string GetSchemaAMITAL_DB(int tenantSeed = 1)
        {
            OracleConnectionStringBuilder csb = null;
            string dbConnectionInfo = "";
            if (AmitalCloudSettings.GetAmitalCustomsSettingsMInject != null)
            {
                dbConnectionInfo = AmitalCloudSettings.GetAmitalCustomsSettingsMInject(tenantSeed).UnfConnectionString;
            }
            else
            {
                dbConnectionInfo = AmitalCloudSettings.GetUnfDBConnectionInfoFromTenantInject(tenantSeed);
            }
            csb = GetOracleConStrBuilder(dbConnectionInfo);
            return csb.UserID.ToUpper();
        }
    }

}
