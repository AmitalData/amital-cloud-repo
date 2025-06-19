using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.Enums;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;

namespace AmitalCloud.Infrastructure.Data.Counters
{
    public class TenantCounter
    {
        private readonly string _dbConnectionInfo;
        public TenantCounter(string dbConnectionInfo)
        {
            _dbConnectionInfo = dbConnectionInfo;
        }

        public int GetNumber()
        {
            int number = 0;

            string strConnString = GetConnection(_dbConnectionInfo);

            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                using (OracleConnection cn = new OracleConnection(strConnString))
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = cn;
                    cmd.CommandText =
                        DBHelpers.DbContextBaseUtil.GetStoredProcedureName("GetNextGlobalTenantId", AmitalCloudDBSchema.AMITAL_GLOBAL,
                        cmd.Connection.ConnectionString);
                    cmd.CommandType = CommandType.StoredProcedure;


                    OracleParameter lastNumberPar = new OracleParameter("v_pLastNumber", OracleDbType.Int32);

                    lastNumberPar.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(lastNumberPar);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                    number = (int)cmd.Parameters["v_pLastNumber"].Value;
                }

                return number;

            }
            else
            {
                using (SqlConnection cn = new SqlConnection(strConnString))
                {
                    SqlCommand cmd = new SqlCommand("GetNextGlobalTenantId", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter lastNumberPar = new SqlParameter("@pLastNumber", SqlDbType.Int);

                    lastNumberPar.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(lastNumberPar);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                    number = (int)cmd.Parameters["@pLastNumber"].Value;

                }
            }
            return number;
        }


        public static string GetConnection(string dbConnectioninfo)
        {
            IGlobalContext context = GlobalContext.GetContext();
            return context.Database.GetDbConnection().ConnectionString;
        }

    }
}