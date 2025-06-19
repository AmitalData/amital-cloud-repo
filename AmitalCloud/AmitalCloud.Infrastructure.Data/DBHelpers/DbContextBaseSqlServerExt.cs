using AmitalCloud.Infrastructure.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.DBHelpers
{
    internal static class DbContextBaseSqlServerExt
    {
        /// <summary> 
        /// Execute stored procedure with single table value parameter. 
        /// </summary> 
        /// <typeparam name="T">Type of object to store.</typeparam> 
        /// <param name="context">DbContext instance.</param> 
        /// <param name="data">Data to store</param> 
        /// <param name="procedureName">Procedure name</param> 
        /// <param name="paramName">Parameter name</param> 
        /// <param name="typeName">User table type name</param> 
        public static void ExecuteTableValueProcedure<T>(this DbContext context, IEnumerable<T> data, string procedureName, string paramName, string typeName)
        {
            //// convert source data to DataTable 
            DataTable table = data.ToDataTable();
            //// create parameter 
            SqlParameter parameter = new SqlParameter(paramName, table);
            parameter.SqlDbType = SqlDbType.Structured;
            parameter.TypeName = typeName;
            //// execute sp sql 
            string sql = String.Format("EXEC {0} {1};", procedureName, paramName);
            //// execute sql 
            context.Database.ExecuteSqlRaw(sql, parameter);
        }
        /// <summary> 
        /// Creates data table from source data. 
        /// </summary> 
        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            DataTable table = new DataTable();
            //// get properties of T 
            var binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
            var options = PropertyReflectionOptions.IgnoreEnumerable | PropertyReflectionOptions.IgnoreIndexer;
            var properties = GetProperties<T>(binding, options).ToList();
            //// create table schema based on properties 
            foreach (var property in properties)
            {
                table.Columns.Add(property.Name, property.PropertyType);
            }
            //// create table data from T instances 
            object[] values = new object[properties.Count];
            foreach (T item in source)
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        /// <summary> 
        /// Gets properties of T 
        /// </summary> 
        public static IEnumerable<PropertyInfo> GetProperties<T>(BindingFlags binding, PropertyReflectionOptions options = PropertyReflectionOptions.All)
        {
            var properties = typeof(T).GetProperties(binding);
            bool all = (options & PropertyReflectionOptions.All) != 0;
            bool ignoreIndexer = (options & PropertyReflectionOptions.IgnoreIndexer) != 0;
            bool ignoreEnumerable = (options & PropertyReflectionOptions.IgnoreEnumerable) != 0;
            foreach (var property in properties)
            {
                if (!all)
                {
                    if (ignoreIndexer && IsIndexer(property))
                    {
                        continue;
                    }

                    if (ignoreIndexer && !property.PropertyType.Equals(typeof(string)) && IsEnumerable(property))
                    {
                        continue;
                    }
                }
                yield return property;
            }
        }
        /// <summary> 
        /// Check if property is indexer 
        /// </summary> 
        private static bool IsIndexer(PropertyInfo property)
        {
            var parameters = property.GetIndexParameters();
            if (parameters != null && parameters.Length > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary> 
        /// Check if property implements IEnumerable 
        /// </summary> 
        private static bool IsEnumerable(PropertyInfo property)
        {
            return property.PropertyType.GetInterfaces().Any(x => x.Equals(typeof(System.Collections.IEnumerable)));
        }
    }
}
