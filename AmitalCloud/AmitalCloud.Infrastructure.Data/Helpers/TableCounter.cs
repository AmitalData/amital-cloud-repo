using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.DBHelpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Enums;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public partial class TableCounter
    {
        private static Object thisLock = new Object();
        public static string GetNumber(int tenant, string counterCode, string parameter1, string parameter2, Dictionary<string, string> additionalParameters = null)
        {
            CounterDefinition counterDef = null;
            List<CounterDefinition> tableCounters = null;
            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                tableCounters = new Repository<CounterDefinition>(tenant).GetMulti(c => c.Tenant == tenant && c.Counter.Code == counterCode).ToList();
                bool isCustomizedCounter = tableCounters.Where(c => c.IsCustomized).Any();
                if (isCustomizedCounter && FeatureToggleHelper.HasFeatureToggle("ICC", tenant) && additionalParameters != null && additionalParameters.ContainsKey("[CustomizeCounterParameter2]") && !string.IsNullOrEmpty(additionalParameters["[CustomizeCounterParameter2]"]))
                {
                    counterDef = tableCounters.Where(c => c.IsCustomized && c.Parameter2 == additionalParameters["[CustomizeCounterParameter2]"]).FirstOrDefault();
                }
                else counterDef = tableCounters.Where(c => c.Parameter1 == parameter1 && c.Parameter2 == parameter2).FirstOrDefault();
                scope.Complete();
            }
            string prefix = null;
            if (counterDef.UniquePerPrefix)
            {
                prefix = counterDef.Prefix;
            }

            string branchCounterCode = null;
            if (FeatureToggleHelper.HasFeatureToggle("SPB", tenant) && counterDef.UsePerBranch && additionalParameters != null)
            {
                ValidateBranchCounterCode(additionalParameters);
                branchCounterCode = additionalParameters["[B]"];
            }

            int startNumber = counterDef.StartNumber;
            string number = null;
            string strConnString = GetConnection(tenant);//ConfigurationManager.ConnectionStrings["str"].ConnectionString;
            string counterLastNumberValue = string.Empty;
            if (FeatureToggleHelper.HasFeatureToggle("LCP", tenant))
            {
                lock (thisLock)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewReadCommittedTransaction())
                    {
                        counterLastNumberValue = ExecuteNextTableNumberValueProcedure(tenant, counterDef.CounterId, prefix, startNumber, strConnString, branchCounterCode);
                        scope.Complete();
                    }
                }
            }
            else
            {
                counterLastNumberValue = ExecuteNextTableNumberValueProcedure(tenant, counterDef.CounterId, prefix, startNumber, strConnString, branchCounterCode);
            }

            number = GetCounterLastNumberWithPrefixSuffix(counterDef, tenant, counterLastNumberValue, additionalParameters);

            return number;
        }
        private static void ValidateBranchCounterCode(Dictionary<string, string> additionalParameters)
        {
            if (string.IsNullOrEmpty(additionalParameters["[BranchName]"]))
            {
                throw new Exception("Branch Field is required");
            }
            if (string.IsNullOrEmpty(additionalParameters["[B]"]))
            {
                throw new Exception("The Counter Code of the " + additionalParameters["[BranchName]"] + " Branch is required.");
            }
        }
        private static string ExecuteNextTableNumberValueProcedure(int tenant, string counterId, string prefix, int startNumber, string strConnString, string branchCounterCode)
        {
#if false
            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                return TableCounter.ExecuteNextTableNumberValueProcedureCustoms(tenant, counter, prefix, startNumber, strConnString);
            }
#endif
            string counterLastNumberValue;
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                using (OracleConnection cn = new OracleConnection(strConnString))
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = cn;
                    cmd.CommandText =
                        DbContextBaseUtil.GetStoredProcedureName("usp_GetNextTableNumberValue", AmitalCloudDBSchema.AMITAL_MAIN,
                        cmd.Connection.ConnectionString);
                    cmd.CommandType = CommandType.StoredProcedure;
                    /*
                     v_pLastValue OUT NUMBER,
--    v_pTenant      IN NUMBER,
--    v_pCounterId   IN NVARCHAR2,
--    v_pPrefix      IN NVARCHAR2,
--    v_pStartNumber IN NUMBER */
                    try
                    {
                        OracleParameter lastValuePar = new OracleParameter("v_pLastValue", OracleDbType.Int32);
                        OracleParameter counterIdPar = new OracleParameter("v_pCounterId", OracleDbType.NVarchar2);
                        OracleParameter tenantPar = new OracleParameter("v_pTenant", OracleDbType.Int32);
                        OracleParameter prefixPar = new OracleParameter("v_pPrefix", OracleDbType.NVarchar2);
                        OracleParameter startNumberPar = new OracleParameter("v_pStartNumber", OracleDbType.Int32);

                        lastValuePar.Direction = ParameterDirection.Output;
                        counterIdPar.Direction = ParameterDirection.Input;
                        tenantPar.Direction = ParameterDirection.Input;
                        prefixPar.Direction = ParameterDirection.Input;
                        startNumberPar.Direction = ParameterDirection.Input;

                        counterIdPar.Value = counterId;
                        tenantPar.Value = tenant;

                        startNumberPar.Value = startNumber;

                        if (prefix != null)
                        {
                            prefixPar.Value = prefix;
                        }
                        else
                        {
                            prefixPar.Value = DBNull.Value;
                        }

                        cmd.Parameters.Add(lastValuePar);
                        cmd.Parameters.Add(tenantPar);
                        cmd.Parameters.Add(counterIdPar);
                        cmd.Parameters.Add(prefixPar);
                        cmd.Parameters.Add(startNumberPar);


                        cn.Open();
                        cmd.ExecuteNonQuery();
                        cn.Close();
                        //number = (counterDef.Prefix != null ? counterDef.Prefix + cmd.Parameters["v_pLastValue"].Value : counterDef.Prefix + cmd.Parameters["v_pLastValue"].Value);
                        counterLastNumberValue = cmd.Parameters["v_pLastValue"].Value.ToString();


                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception: {0}", ex.ToString());
                        throw;
                    }

                    cn.Close();
                }
            }
            else
            {
                using (SqlConnection cn = new SqlConnection(strConnString))
                {
                    //SqlCommand cmd = new SqlCommand("dbo.usp_GetNextTableNumberValue", cn);
                    //List<int> newSPTenants = new List<int>()
                    //{
                    //    1,42,1330,2573
                    //};
                    //if (newSPTenants.Where(a=>a == tenant).Count() > 0)
                    //{
                    string procedureName = string.IsNullOrEmpty(branchCounterCode) ? "dbo.usp_GetNextTableNumberValueWithSnapshotView" : "dbo.usp_GetNextTableNumberValueSupportBranchCounterCodeWithSnapshot";
                    SqlCommand cmd = new SqlCommand(procedureName, cn);
                    //}
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter lastValuePar = new SqlParameter("@pLastValue", SqlDbType.Int);
                    SqlParameter counterIdPar = new SqlParameter("@pCounterId", SqlDbType.NVarChar);
                    SqlParameter tenantPar = new SqlParameter("@pTenant", SqlDbType.Int);
                    SqlParameter prefixPar = new SqlParameter("@pPrefix", SqlDbType.NVarChar);
                    SqlParameter startNumberPar = new SqlParameter("@pStartNumber", SqlDbType.Int);

                    lastValuePar.Direction = ParameterDirection.Output;
                    counterIdPar.Direction = ParameterDirection.Input;
                    tenantPar.Direction = ParameterDirection.Input;
                    prefixPar.Direction = ParameterDirection.Input;
                    startNumberPar.Direction = ParameterDirection.Input;

                    counterIdPar.Value = counterId;
                    tenantPar.Value = tenant;

                    startNumberPar.Value = startNumber;

                    if (prefix != null)
                    {
                        prefixPar.Value = prefix;
                    }
                    else
                    {
                        prefixPar.Value = DBNull.Value;
                    }


                    cmd.Parameters.Add(lastValuePar);
                    cmd.Parameters.Add(tenantPar);
                    cmd.Parameters.Add(prefixPar);
                    cmd.Parameters.Add(counterIdPar);
                    cmd.Parameters.Add(startNumberPar);

                    AddbranchCounterCodeParameter(branchCounterCode, cmd);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                    cn.Close();
                    //number = (counterDef.Prefix != null ? counterDef.Prefix + cmd.Parameters["@pLastValue"].Value : counterDef.Prefix + cmd.Parameters["@pLastValue"].Value);
                    counterLastNumberValue = cmd.Parameters["@pLastValue"].Value.ToString();


                }
            }

            return counterLastNumberValue;
        }

        private static void AddbranchCounterCodeParameter(string branchCounterCode, SqlCommand cmd)
        {
            if (string.IsNullOrEmpty(branchCounterCode)) return;
            SqlParameter branchCounterCodePar = new SqlParameter("@pBranchCounterCode", SqlDbType.VarChar);
            branchCounterCodePar.Direction = ParameterDirection.Input;
            if (branchCounterCode != null)
            {
                branchCounterCodePar.Value = branchCounterCode;
            }
            else
            {
                branchCounterCodePar.Value = DBNull.Value;
            }
            cmd.Parameters.Add(branchCounterCodePar);
        }

        private static string GetCounterLastNumberWithPrefixSuffix(CounterDefinition counterDef, int tenant, string counterLastNumberValue, Dictionary<string, string> additionalParameters)
        {
            string counterPrefix = !string.IsNullOrEmpty(counterDef.Prefix) ? counterDef.Prefix : "";
            string counterSuffix = !string.IsNullOrEmpty(counterDef.Suffix) ? counterDef.Suffix : "";


            //if (!string.IsNullOrEmpty(counterPrefix))
            //{
            //number = (counterDef.Prefix != null ? counterDef.Prefix + cmd.Parameters["v_pLastValue"].Value : counterDef.Prefix + cmd.Parameters["v_pLastValue"].Value);

            //[MM],[YY] or [YYYY],[B]
            ResolveCounterPrefixSuffixVariables(tenant, additionalParameters, ref counterPrefix, ref counterSuffix);
            //YYYShipEEE (15 - 9) + 3 
            if (counterDef.CounterSize != null && counterDef.CounterSize.Value > 0 && (counterPrefix + counterLastNumberValue + counterSuffix).Length < counterDef.CounterSize.Value)
            {
                int sizeOfStartNumber = (counterDef.CounterSize.Value - (counterPrefix + counterSuffix).Length);
                counterLastNumberValue = counterLastNumberValue.ToString().PadLeft(sizeOfStartNumber, '0');
            }

            counterLastNumberValue = counterPrefix + counterLastNumberValue + counterSuffix;

            return counterLastNumberValue;
        }

        private static void ResolveCounterPrefixSuffixVariables(int tenant, Dictionary<string, string> additionalParameters, ref string counterPrefix, ref string counterSuffix)
        {
            DateTime date = TenantServerConfigration.GetCurrentDateTime(tenant);
            string MM = date.ToString("MM");
            string YY = date.ToString("yy");
            string YYYY = date.ToString("yyyy");

            counterPrefix = counterPrefix.Replace("[MM]", MM).Replace("[YY]", YY).Replace("[YYYY]", YYYY);
            counterSuffix = counterSuffix.Replace("[MM]", MM).Replace("[YY]", YY).Replace("[YYYY]", YYYY);
            if (additionalParameters != null)
            {
                foreach (var k in additionalParameters.Keys)
                {
                    if (k == "[B]" && !FeatureToggleHelper.HasFeatureToggle("BCC", tenant))
                    {
                        continue;
                    }

                    counterPrefix = counterPrefix.Replace(k, additionalParameters[k]);
                    counterSuffix = counterSuffix.Replace(k, additionalParameters[k]);
                }
            }

        }

        public static string GetCounterPrefix(int tenant, string counterCode, string parameter1, string parameter2, Dictionary<string, string> additionalParameters = null)
        {
            CounterDefinition counterDef = new Repository<CounterDefinition>(tenant)
                .GetMulti(c => c.Tenant == tenant && c.Counter.Code == counterCode && c.Parameter1 == parameter1 && c.Parameter2 == parameter2).FirstOrDefault();
            string counterPrefix = !string.IsNullOrEmpty(counterDef.Prefix) ? counterDef.Prefix : "";
            string counterSuffix = !string.IsNullOrEmpty(counterDef.Suffix) ? counterDef.Suffix : "";
            ResolveCounterPrefixSuffixVariables(tenant, additionalParameters, ref counterPrefix, ref counterSuffix);
            return counterPrefix;
        }

        public static string GetConnection(int tenant)
        {
            GlobalDB currentDb;
            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                currentDb = new GlobalDBRepository(ConfigurationHelper.Conf).GetGlobalDBByTenant(tenant);

            }
            string dbConnectionInfo = currentDb.DBConnection;
            string dbSeconderyConnectionInfo = currentDb.SecondaryAzureDBConnection;
            DbConnection connection = DatabaseInitializer.GetConnection(dbConnectionInfo, dbSeconderyConnectionInfo);
            IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
            return context.Database.GetDbConnection().ConnectionString;
        }
    }
}
