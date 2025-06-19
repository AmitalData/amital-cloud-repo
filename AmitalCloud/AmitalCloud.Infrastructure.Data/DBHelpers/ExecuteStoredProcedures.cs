using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace AmitalCloud.Infrastructure.Data.DBHelpers
{
    public class ExecuteStoredProcedures
    {
        public static object Execute(string procedureName, int tenant, List<StoredProcedureParam> storedProcedureParams)
        {
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                if (procedureName.Contains("dbo."))
                {
                    procedureName = procedureName.Split('.')[1];
                }
                return ExecuteOracle(procedureName, tenant, storedProcedureParams);
            }
            else
            {
                return ExecuteMMSQL(procedureName, tenant, storedProcedureParams);
            }
        }
        public static string GetConnection(int tenant)
        {
            return AmitalCloudContext.GetContext(tenant).Database.GetDbConnection().ConnectionString;
        }
        static object ExecuteOracle(string procedureName, int tenant, List<StoredProcedureParam> storedProcedureParams)
        {
            string strConnString = GetConnection(tenant);
            object outValue = null;
            using (OracleConnection cn = new OracleConnection(strConnString))
            {
                var cmd = new OracleCommand();
                cmd.Connection = cn;
                cmd.CommandText = // "usp_UpdateQueueCommunicationLo";
                    procedureName.Substring(0, Math.Min(procedureName.Length, 30));
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (StoredProcedureParam parameter in storedProcedureParams)
                {
                    OracleParameter param = ToOracleParameter(parameter);
                    param.Direction = parameter.Direction;
                    if (parameter.Value != null)
                    {
                        param.Value = parameter.Value;
                    }
                    cmd.Parameters.Add(param);
                }
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
                StoredProcedureParam outParam = storedProcedureParams.Where(d => d.Direction == ParameterDirection.Output).FirstOrDefault();
                if (outParam != null)
                {
                    outValue = cmd.Parameters[outParam.ParamName].Value;
                }
            }
            return outValue;
        }
        private static OracleParameter ToOracleParameter(StoredProcedureParam parameter)
        {
            var oracleParameter = new OracleParameter();
            return oracleParameter;
        }
        private static string ToOracleParamName(string p)
        {
            throw new NotImplementedException();
        }
        static object ExecuteMMSQL(string procedureName, int tenant, List<StoredProcedureParam> storedProcedureParams)
        {
            string strConnString = GetConnection(tenant);
            object outValue = null;
            using (SqlConnection cn = new SqlConnection(strConnString))
            {
                SqlCommand cmd = new SqlCommand(procedureName, cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 1200;
                foreach (StoredProcedureParam parameter in storedProcedureParams)
                {
                    SqlParameter param = new SqlParameter(parameter.ParamName, parameter.ParamDBType, parameter.ParamSize);
                    param.Direction = parameter.Direction;
                    if (parameter.Value != null)
                    {
                        param.Value = parameter.Value;
                    }
                    cmd.Parameters.Add(param);
                }
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
                StoredProcedureParam outParam = storedProcedureParams.Where(d => d.Direction == ParameterDirection.Output).FirstOrDefault();
                if (outParam != null)
                {
                    outValue = cmd.Parameters[outParam.ParamName].Value;
                }
            }
            return outValue;
        }
    }

}