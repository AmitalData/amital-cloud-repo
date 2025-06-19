using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Transactions;

namespace AmitalCloud.Infrastructure.Data.Counters
{
    public class IdCounter //: IIdCounter
    {

        public static IIdCounter FakeOverrideIIdCounter { get; set; }

        public static string GetNumber(string connectionString, string tableName)
        {
            if (FakeOverrideIIdCounter != null)
            {
                return FakeOverrideIIdCounter.GetNumber(connectionString, tableName);
            }
            string number = null;
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                //  throw new Exception("itzik+elisheva= not in GetNewReadCommittedTransaction !!!!!! ");

                // ALL IdCounter MUST BE IN TRANSACTION 
                // ALTHOGH THIS FUNCTION CALL FRM UNITEST 
                using (TransactionScope scope = TransactionFactory.GetNewReadCommittedTransaction())
                {

                    using (OracleConnection cn = new OracleConnection(connectionString))
                    {
                        OracleCommand cmd = new OracleCommand();
                        cmd.Connection = cn;
                        cmd.CommandText =
                    DBHelpers.DbContextBaseUtil.GetStoredProcedureName("usp_GetNextTableIdValue", AmitalCloudDBSchema.AMITAL_MAIN,
                        cmd.Connection.ConnectionString);
                        cmd.CommandType = CommandType.StoredProcedure;
                        /*
                          v_pLastNumber OUT VARCHAR2,
    --                    v_pTableName IN VARCHAR2 
                         * */

                        OracleParameter lastNumberPar = new OracleParameter("v_pLastNumber", OracleDbType.Varchar2, 100);
                        OracleParameter tableNamePar = new OracleParameter("v_pTableName", OracleDbType.Varchar2);

                        lastNumberPar.Direction = ParameterDirection.Output;
                        tableNamePar.Direction = ParameterDirection.Input;

                        tableNamePar.Value = tableName;

                        cmd.Parameters.Add(lastNumberPar);
                        cmd.Parameters.Add(tableNamePar);
                        try
                        {
                            cn.Open();
                            cmd.ExecuteNonQuery();
                            number = (string)cmd.Parameters["v_pLastNumber"].Value;

                        }
                        catch (Exception ex)
                        {
                            System.Console.WriteLine("Exception: {0}", ex.ToString());
                            throw;
                        }

                        cn.Close();
                    }
                    scope.Complete();
                }
                return number;
            }

            else
            {

                string strConnString = connectionString;
                using (SqlConnection cn = new SqlConnection(strConnString))
                {


                    SqlParameter lastNumberPar = new SqlParameter("@v_pLastNumber", SqlDbType.VarChar, 100);
                    SqlParameter tableNamePar = new SqlParameter("@v_pTableName", SqlDbType.VarChar);

                    lastNumberPar.Direction = ParameterDirection.Output;
                    tableNamePar.Direction = ParameterDirection.Input;

                    tableNamePar.Value = tableName;

                    SqlCommand cmd = new SqlCommand("usp_GetNextTableIdValue", cn);
                    cmd.CommandType = CommandType.StoredProcedure;


                    cmd.Parameters.Add(lastNumberPar);
                    cmd.Parameters.Add(tableNamePar);
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                    number = (string)cmd.Parameters["@v_pLastNumber"].Value;

                }
                // scope.Complete();
                //}
                return number;

            }
        }

        private static Dictionary<string, Queue<string>> TablesIdsRange = new Dictionary<string, Queue<string>>();
        private static Queue<string> IdStoreQueue = new Queue<string>();

        private static SqlCommand cmd;
        //private static Queue<string> tableIdsQueue = new Queue<string>();

        // private static Dictionary<string, Queue<string>> TablesIdsRange = new Dictionary<string, Queue<string>>();
        public static string GetIdWithIdsRange(string tableName, int numberOfIds, int tenant)
        {

            if (TablesIdsRange.ContainsKey(tableName) && TablesIdsRange[tableName].Count > 0)
            {
                string id = TablesIdsRange[tableName].Dequeue();
                return id;
            }
            else
            {
                string strConnString = GetConnection(tenant);
                int startNumber;
                int endNumber;
                string dbString;

                if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                {

                    lock (thisLock)
                        using (TransactionScope scope = TransactionFactory.GetNewReadCommittedTransaction())
                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();

                            // NOWAIT ///cmd.CommandTimeout = 4;//The time in seconds to wait for the command to execute. The default is 30 seconds.



                            cmd.Connection = cn;
                            cmd.CommandText =
                            //Enums.AmitalCloudDBSchema.AMITAL_MAIN.ToString() + "." +   "usp_GetNextTableIdValue";
                            DBHelpers.DbContextBaseUtil.GetStoredProcedureName("usp_GetNextTableIdsRange", AmitalCloudDBSchema.AMITAL_MAIN,
                            cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;

                            OracleParameter startNumberPar = new OracleParameter("v_pStartNumber", OracleDbType.Int32);
                            OracleParameter endNumberPar = new OracleParameter("v_pEndNumber", OracleDbType.Int32);
                            OracleParameter dbStringNumber = new OracleParameter("v_DBStringNumber", OracleDbType.Varchar2, 50);
                            OracleParameter tableNamePar = new OracleParameter("v_pTableName", OracleDbType.Varchar2);
                            OracleParameter numberOfIdsPar = new OracleParameter("v_pNumberOfIds", OracleDbType.Int32);


                            startNumberPar.Direction = ParameterDirection.Output;
                            endNumberPar.Direction = ParameterDirection.Output;
                            dbStringNumber.Direction = ParameterDirection.Output;
                            tableNamePar.Direction = ParameterDirection.Input;
                            numberOfIdsPar.Direction = ParameterDirection.Input;

                            tableNamePar.Value = tableName;
                            numberOfIdsPar.Value = numberOfIds;


                            cmd.Parameters.Add(startNumberPar);
                            cmd.Parameters.Add(numberOfIdsPar);
                            cmd.Parameters.Add(endNumberPar);
                            cmd.Parameters.Add(tableNamePar);
                            cmd.Parameters.Add(dbStringNumber);

                            try
                            {
                                cn.Open();
                                cmd.ExecuteNonQuery();

                                startNumber = (int)cmd.Parameters["v_pStartNumber"].Value;
                                endNumber = (int)cmd.Parameters["v_pEndNumber"].Value;
                                dbString = (string)cmd.Parameters["v_DBStringNumber"].Value;


                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                throw;
                            }
                            scope.Complete();
                            cn.Close();
                        }


                }
                else
                {

                    lock (thisLock)
                    {
                        using (TransactionScope scope = TransactionFactory.GetNewReadCommittedTransaction())
                        {
                            using (SqlConnection cn = new SqlConnection(strConnString))
                            {

                                SqlCommand cmd = new SqlCommand("usp_GetNextTableIdsRange", cn);
                                cmd.CommandType = CommandType.StoredProcedure;

                                SqlParameter startNumberPar = new SqlParameter("@pStartNumber", SqlDbType.Int);
                                SqlParameter endNumberPar = new SqlParameter("@pEndNumber", SqlDbType.Int);
                                SqlParameter dbStringNumber = new SqlParameter("@DBStringNumber", SqlDbType.VarChar, 50);
                                SqlParameter tableNamePar = new SqlParameter("@pTableName", SqlDbType.VarChar);
                                SqlParameter numberOfIdsPar = new SqlParameter("@pNumberOfIds", SqlDbType.Int);


                                startNumberPar.Direction = ParameterDirection.Output;
                                endNumberPar.Direction = ParameterDirection.Output;
                                dbStringNumber.Direction = ParameterDirection.Output;
                                tableNamePar.Direction = ParameterDirection.Input;
                                numberOfIdsPar.Direction = ParameterDirection.Input;

                                tableNamePar.Value = tableName;
                                numberOfIdsPar.Value = numberOfIds;


                                cmd.Parameters.Add(startNumberPar);
                                cmd.Parameters.Add(numberOfIdsPar);
                                cmd.Parameters.Add(endNumberPar);
                                cmd.Parameters.Add(tableNamePar);
                                cmd.Parameters.Add(dbStringNumber);
                                cn.Open();
                                cmd.ExecuteNonQuery();
                                cn.Close();

                                startNumber = (int)cmd.Parameters["@pStartNumber"].Value;
                                endNumber = (int)cmd.Parameters["@pEndNumber"].Value;
                                dbString = (string)cmd.Parameters["@DBStringNumber"].Value;

                                scope.Complete();
                            }
                        }
                    }
                }


                Queue<string> tableIdsQueue = new Queue<string>();
                for (int i = startNumber; i <= endNumber; i++)
                {
                    tableIdsQueue.Enqueue(dbString + "-" + i);
                }


                string id = tableIdsQueue.Dequeue();

                if (TablesIdsRange.Keys.Contains(tableName))
                {
                    TablesIdsRange[tableName] = tableIdsQueue;
                }
                else
                {
                    TablesIdsRange.Add(tableName, tableIdsQueue);
                }

                return id;
            }
        }
        private static Object thisLock = new Object();
        public static string GetNumber(string tableName, int tenant)
        {
            if (FakeOverrideIIdCounter != null)
            {
                return FakeOverrideIIdCounter.GetNumber(tableName, tenant);
            }
            string number = null;
            string strConnString = GetConnection(tenant);
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {

                var sw = Stopwatch.StartNew();
                lock (thisLock)
                    using (TransactionScope scope = TransactionFactory.GetNewReadCommittedTransaction())
                    {
                        using (OracleConnection cn = new OracleConnection(strConnString))
                        {
                            OracleCommand cmd = new OracleCommand();
                            cmd.Connection = cn;
                            cmd.CommandText =
                            //Enums.AmitalCloudDBSchema.AMITAL_MAIN.ToString() + "." +   "usp_GetNextTableIdValue";
                            DBHelpers.DbContextBaseUtil.GetStoredProcedureName("usp_GetNextTableIdValue", AmitalCloudDBSchema.AMITAL_MAIN,
                            cmd.Connection.ConnectionString);
                            cmd.CommandType = CommandType.StoredProcedure;
                            /*
                              v_pLastNumber OUT VARCHAR2,
                   --                    v_pTableName IN VARCHAR2 
                             * */

                            OracleParameter lastNumberPar = new OracleParameter("v_pLastNumber", OracleDbType.Varchar2, 100);
                            OracleParameter tableNamePar = new OracleParameter("v_pTableName", OracleDbType.Varchar2);

                            lastNumberPar.Direction = ParameterDirection.Output;
                            tableNamePar.Direction = ParameterDirection.Input;

                            tableNamePar.Value = tableName;

                            cmd.Parameters.Add(lastNumberPar);
                            cmd.Parameters.Add(tableNamePar);
                            try
                            {
                                cn.Open();
                                cmd.ExecuteNonQuery();
                                number = (string)cmd.Parameters["v_pLastNumber"].Value;

                            }
                            catch (Exception ex)
                            {
                                System.Console.WriteLine("Exception: {0}", ex.ToString());
                                // scope.Dispose();
                                //cn.Close();

                                throw;
                            }
                            scope.Complete();
                            cn.Close();
                        }
                    }

                LogMessagingUtil.Instance.AppendLine($"GetNumber({tableName}):took:{sw.Elapsed}");
                return number;
            }

            else
            {
                TransactionScope scope = null;
                try
                {
                    using (scope = TransactionFactory.GetNewReadCommittedTransaction(TimeSpan.FromSeconds(3)))
                    {
                        using (SqlConnection cn = new SqlConnection(strConnString))
                        {
                            SqlParameter lastNumberPar = null;
                            SqlParameter tableNamePar = null;


                            //if (AmitalCloudSettings.IsCostomsDeploy)
                            //{
                            //    lastNumberPar = new SqlParameter("@v_pLastNumber", SqlDbType.VarChar, 100);
                            //    tableNamePar = new SqlParameter("@v_pTableName", SqlDbType.VarChar);

                            //}
                            //else
                            //{
                            lastNumberPar = new SqlParameter("@V_PLASTNUMBER", SqlDbType.VarChar, 100);
                            tableNamePar = new SqlParameter("@v_pTableName", SqlDbType.VarChar);

                            //}

                            lastNumberPar.Direction = ParameterDirection.Output;
                            tableNamePar.Direction = ParameterDirection.Input;

                            tableNamePar.Value = tableName;

                            SqlCommand cmd = new SqlCommand("usp_GetNextTableIdValue", cn);
                            cmd.CommandType = CommandType.StoredProcedure;


                            cmd.Parameters.Add(lastNumberPar);
                            cmd.Parameters.Add(tableNamePar);
                            //cmd.CommandTimeout = 3;
                            cn.Open();
                            cmd.ExecuteNonQuery();
                            cn.Close();
                            //if (AmitalCloudSettings.IsCostomsDeploy)
                            //{
                            number = (string)cmd.Parameters["@v_pLastNumber"].Value;

                            //}
                            //else
                            //{
                            //    number = (string)cmd.Parameters["@pLastNumber"].Value;

                            //}


                        }

                        scope.Complete();


                        return number;
                    }
                }
                catch (Exception ex)
                {
                    //if (scope != null)
                    //{
                    //    scope.Dispose();
                    //}
                    throw ex;
                }



            }
        }

        //************************************************************************************************************************************
        //public
        static string GetConnection(int tenant)
        {
            return AmitalCloudContext.GetContext(tenant).Database.GetDbConnection().ConnectionString;
        }


    }
    public interface IIdCounter
    {
        string GetNumber(string tableName, int tenant);

        string GetNumber(string connectionString, string tableName);
    }

    public class IdCounterWrapper : IIdCounter
    {
        public string GetNumber(string tableName, int tenant)
        {
            return IdCounter.GetNumber(tableName, tenant);
        }

        public string GetNumber(string connectionString, string tableName)
        {
            return IdCounter.GetNumber(connectionString, tableName);
        }
    }
    public class FakeIdCounter : IIdCounter
    {
        public Func<string, int, string> GetNumberBytableNameStringtenantInt { get; set; }
        public string GetNumber(string tableName, int tenant)
        {
            return GetNumberBytableNameStringtenantInt(tableName, tenant);
        }

        public Func<string, string, string> GetNumberBystringConnectionStringStringtableName { get; set; }
        public string GetNumber(string connectionString, string tableName)
        {
            return GetNumberBystringConnectionStringStringtableName(connectionString, tableName);
        }
    }
}



