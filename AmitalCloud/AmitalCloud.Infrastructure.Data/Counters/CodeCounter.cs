using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.Enums;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace AmitalCloud.Infrastructure.Data.Counters
{
    public class CodeCounter
    {
        private static Object thisLock = new Object();
        public static int GetNumber(string tableName, int tenant, bool InOracleCreateNewTransaction = false)
        {
            int number = 0;
            string strConnString = GetConnection(tenant);

            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                if (InOracleCreateNewTransaction)
                {
                    LogMessagingUtil.Instance.AppendLine($"GetCodeValueFromOracle({tableName}-InOracleCreateNewTransaction");
                    using (var scope = TransactionFactory.GetNewTransaction())
                    {
                        number = GetCodeValueFromOracle(tableName, tenant, strConnString);
                        scope.Complete();
                    }
                }
                else
                {
                    number = GetCodeValueFromOracle(tableName, tenant, strConnString);
                }


                return number;
            }
            else
            {
                int Retry = 0;
                try
                {

                    using (TransactionScope scope = TransactionFactory.GetNewTransaction(TimeSpan.FromSeconds(3)))
                    {

                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {

                            SqlCommand cmd = new SqlCommand("dbo.usp_GetNextTableCodeValueWithSnapShot", cn);

                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter lastNumberPar = new SqlParameter("@pLastNumber", SqlDbType.Int);
                            SqlParameter tableNamePar = new SqlParameter("@pTableName", SqlDbType.NVarChar);
                            SqlParameter tenantPar = new SqlParameter("@pTenant", SqlDbType.Int);


                            lastNumberPar.Direction = ParameterDirection.Output;
                            tableNamePar.Direction = ParameterDirection.Input;
                            tenantPar.Direction = ParameterDirection.Input;

                            tenantPar.Value = tenant;
                            tableNamePar.Value = tableName;

                            cmd.Parameters.Add(lastNumberPar);
                            cmd.Parameters.Add(tenantPar);
                            cmd.Parameters.Add(tableNamePar);
                            cn.Open();
                            cmd.ExecuteNonQuery();
                            cn.Close();
                            number = (int)cmd.Parameters["@pLastNumber"].Value;

                        }

                        scope.Complete();
                        return number;
                    }
                }
                catch (Exception ex)
                {
                    if (Retry == 0)
                    {
                        Retry++;
                        return GetNumber(tableName, tenant);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }


        }

        private static int GetCodeValueFromOracle(string tableName, int tenant, string strConnString)
        {
            int number;
            using (OracleConnection cn = new OracleConnection(strConnString))
            {
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = cn;
                cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("usp_GetNextTableCodeValue", AmitalCloudDBSchema.AMITAL_MAIN,
                    cmd.Connection.ConnectionString);
                cmd.CommandType = CommandType.StoredProcedure;
                /*
                  v_pLastNumber OUT NUMBER,
--    v_pTableName IN NVARCHAR2,
--    v_pTenant    IN NUMBER )
                 * */
                try
                {
                    OracleParameter lastNumberPar = new OracleParameter("v_pLastNumber", OracleDbType.Int32);
                    OracleParameter tableNamePar = new OracleParameter("v_pTableName", OracleDbType.Varchar2);
                    OracleParameter tenantPar = new OracleParameter("v_pTenant", OracleDbType.Int32);


                    lastNumberPar.Direction = ParameterDirection.Output;
                    tableNamePar.Direction = ParameterDirection.Input;
                    tenantPar.Direction = ParameterDirection.Input;

                    tenantPar.Value = tenant;
                    tableNamePar.Value = tableName;

                    cmd.Parameters.Add(lastNumberPar);
                    cmd.Parameters.Add(tableNamePar);
                    cmd.Parameters.Add(tenantPar);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                    number = Convert.ToInt32(cmd.Parameters["v_pLastNumber"].Value);

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("Exception: {0}", ex.ToString());
                    throw;
                }

                cn.Close();
            }

            return number;
        }

        public static int GetNumber_Ticket(string tableName, int tenant)
        {
            int number = 0;
            string strConnString = GetConnection(tenant);
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {

                using (OracleConnection cn = new OracleConnection(strConnString))
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = cn;
                    cmd.CommandText = DBHelpers.DbContextBaseUtil.GetStoredProcedureName("usp_GetNextTableCodeValue", AmitalCloudDBSchema.AMITAL_MAIN,
                        cmd.Connection.ConnectionString);
                    cmd.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        OracleParameter lastNumberPar = new OracleParameter("v_pLastNumber", OracleDbType.Int32);
                        OracleParameter tableNamePar = new OracleParameter("v_pTableName", OracleDbType.Varchar2);
                        OracleParameter tenantPar = new OracleParameter("v_pTenant", OracleDbType.Int32);


                        lastNumberPar.Direction = ParameterDirection.Output;
                        tableNamePar.Direction = ParameterDirection.Input;
                        tenantPar.Direction = ParameterDirection.Input;

                        tenantPar.Value = tenant;
                        tableNamePar.Value = tableName;

                        cmd.Parameters.Add(lastNumberPar);
                        cmd.Parameters.Add(tableNamePar);
                        cmd.Parameters.Add(tenantPar);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                        number = Convert.ToInt32(cmd.Parameters["v_pLastNumber"].Value);

                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception: {0}", ex.ToString());
                        throw;
                    }

                    cn.Close();
                }

                return number;
            }
            else
            {
                using (SqlConnection cn = new SqlConnection(strConnString))
                {
                    SqlCommand cmd = new SqlCommand("dbo.usp_GetNextTableCodeValue", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter lastNumberPar = new SqlParameter("@pLastNumber", SqlDbType.Int);
                    SqlParameter tableNamePar = new SqlParameter("@pTableName", SqlDbType.NVarChar);
                    SqlParameter tenantPar = new SqlParameter("@pTenant", SqlDbType.Int);

                    lastNumberPar.Direction = ParameterDirection.Output;
                    tableNamePar.Direction = ParameterDirection.Input;
                    tenantPar.Direction = ParameterDirection.Input;

                    tenantPar.Value = tenant;
                    tableNamePar.Value = tableName;

                    cmd.Parameters.Add(lastNumberPar);
                    cmd.Parameters.Add(tenantPar);
                    cmd.Parameters.Add(tableNamePar);
                    cmd.CommandTimeout = 3;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                    number = (int)cmd.Parameters["@pLastNumber"].Value;

                }
                return number;
            }
        }
        public static int GetNumber(string tableName, int tenant, string connectionString)
        {

            int number = 0;


            string strConnString = connectionString;//ConfigurationManager.ConnectionStrings["str"].ConnectionString;
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {

                using (OracleConnection cn = new OracleConnection(strConnString))
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = cn;
                    cmd.CommandText =
                    DBHelpers.DbContextBaseUtil.GetStoredProcedureName("usp_GetNextTableCodeValue", AmitalCloudDBSchema.AMITAL_MAIN,
                    cmd.Connection.ConnectionString);
                    cmd.CommandType = CommandType.StoredProcedure;
                    /*
                      v_pLastNumber OUT NUMBER,
--    v_pTableName IN NVARCHAR2,
--    v_pTenant    IN NUMBER )
                     * */
                    try
                    {
                        OracleParameter lastNumberPar = new OracleParameter("v_pLastNumber", OracleDbType.Int32);
                        OracleParameter tableNamePar = new OracleParameter("v_pTableName", OracleDbType.NVarchar2);
                        OracleParameter tenantPar = new OracleParameter("v_pTenant", OracleDbType.Int32);


                        lastNumberPar.Direction = ParameterDirection.Output;
                        tableNamePar.Direction = ParameterDirection.Input;
                        tenantPar.Direction = ParameterDirection.Input;

                        tenantPar.Value = tenant;
                        tableNamePar.Value = tableName;

                        cmd.Parameters.Add(lastNumberPar);
                        cmd.Parameters.Add(tenantPar);
                        cmd.Parameters.Add(tableNamePar);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                        number = (int)cmd.Parameters["@pLastNumber"].Value;

                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception: {0}", ex.ToString());
                        throw;
                    }

                    cn.Close();
                }

                return number;
            }
            else
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    using (SqlConnection cn = new SqlConnection(strConnString))
                    {
                        SqlCommand cmd = new SqlCommand("dbo.usp_GetNextTableCodeValue", cn);
                        var myTenants = new List<int>() { 1, 42, 2889 };
                        if (tenant < 3000)//myTenants.Contains(tenant))
                        {
                            cmd = new SqlCommand("dbo.usp_GetNextTableCodeValueWithSnapShot", cn);
                        }
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter lastNumberPar = new SqlParameter("@pLastNumber", SqlDbType.Int);
                        SqlParameter tableNamePar = new SqlParameter("@pTableName", SqlDbType.NVarChar);
                        SqlParameter tenantPar = new SqlParameter("@pTenant", SqlDbType.Int);


                        lastNumberPar.Direction = ParameterDirection.Output;
                        tableNamePar.Direction = ParameterDirection.Input;
                        tenantPar.Direction = ParameterDirection.Input;

                        tenantPar.Value = tenant;
                        tableNamePar.Value = tableName;

                        cmd.Parameters.Add(lastNumberPar);
                        cmd.Parameters.Add(tenantPar);
                        cmd.Parameters.Add(tableNamePar);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                        number = (int)cmd.Parameters["@pLastNumber"].Value;

                    }
                    scope.Complete();
                }

                return number;

            }
        }
        public static string GetConnection(int tenant)
        {
            IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
            return context.Database.GetDbConnection().ConnectionString;
        }
    }


    public interface ICodeCounter
    {
        int GetNumber(string tableName, int tenant);
        int GetNumber(string tableName, int tenant, string connectionString);
    }
    public class CodeCounterWrapper : ICodeCounter
    {
        private bool _InNewTransaction;
        private static Object thisLock = new Object();

        public CodeCounterWrapper(bool inNewTransaction)
        {
            _InNewTransaction = inNewTransaction;
        }
        public int GetNumber(string tableName, int tenant)
        {
            if (!_InNewTransaction)
            {
                return CodeCounter.GetNumber(tableName, tenant);
            }
            int res;
            lock (thisLock)
            {
                using (TransactionScope scope = TransactionFactory.GetNewReadCommittedTransaction())
                {
                    res = CodeCounter.GetNumber(tableName, tenant);
                    scope.Complete();

                }
            }
            return res;
        }

        public int GetNumber(string tableName, int tenant, string connectionString)
        {


            if (!_InNewTransaction)
            {
                return CodeCounter.GetNumber(tableName, tenant, connectionString);
            }
            int res;
            lock (thisLock)
            {
                using (TransactionScope scope = TransactionFactory.GetNewReadCommittedTransaction())
                {
                    res = CodeCounter.GetNumber(tableName, tenant, connectionString);
                    scope.Complete();

                }
            }
            return res;
        }
    }

}