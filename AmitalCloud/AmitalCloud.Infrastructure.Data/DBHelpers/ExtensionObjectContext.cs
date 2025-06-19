using AmitalCloud.Infrastructure.Domain.DataContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

namespace AmitalCloud.Infrastructure.Data.DBHelpers
{
    public static class ExtensionObjectContext
    {
        public static int FirstOrDefaultFUNOWAITWhere<T>(this DbContext db, Expression<Func<T, bool>> filter) where T : class
        {
            var entityType = db.Model.FindEntityType(typeof(T));
            var tableName = entityType?.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
                throw new InvalidOperationException($"Could not resolve table name for {typeof(T).Name}");

            if (filter.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal && binaryExpr.Left is MemberExpression member && binaryExpr.Right is ConstantExpression constant)
            {
                string columnName = member.Member.Name;
                string sql = $"SELECT 1 AS MyCount FROM [{tableName}] WHERE [{columnName}] = @p0 FOR UPDATE NOWAIT";
                var param = new SqlParameter("@p0", constant.Value ?? DBNull.Value);

                return db.Database.ExecuteSqlRaw(sql, param);
            }

            throw new NotSupportedException("Only simple equality expressions are supported in this version.");

        }

        public static List<T> GetListNOWAITWhere<T>(this DbContext db, Expression<Func<T, bool>> filter) where T : class
        {
            var entityType = db.Model.FindEntityType(typeof(T));
            var tableName = entityType?.GetTableName();
            if (tableName == null)
                throw new InvalidOperationException("Unable to determine table name for entity.");

            if (filter.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal && binaryExpr.Left is MemberExpression member && binaryExpr.Right is ConstantExpression constant)
            {
                string columnName = member.Member.Name;
                string paramName = "@p0";
                object paramValue = constant.Value ?? DBNull.Value;

                string sql;

                if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                {
                    sql = $"SELECT * FROM [{tableName}] WHERE [{columnName}] = {paramName} FOR UPDATE NOWAIT";
                }
                else
                {
                    sql = $"SELECT * FROM [{tableName}] WITH (UPDLOCK, NOWAIT) WHERE [{columnName}] = {paramName}";
                }

                var param = new SqlParameter(paramName, paramValue);

                return db.Set<T>().FromSqlRaw(sql, param).ToList();
            }

            throw new NotSupportedException("Only simple expressions like x => x.Id == value are supported.");
        }

        public static int DeleteWhere<T>(this DbContext db, Expression<Func<T, bool>> filter) where T : class
        {
            var entityType = db.Model.FindEntityType(typeof(T));
            var tableName = entityType?.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
                throw new InvalidOperationException($"Could not determine table name for {typeof(T).Name}");

            if (filter.Body is BinaryExpression binaryExpr && binaryExpr.NodeType == ExpressionType.Equal && binaryExpr.Left is MemberExpression member && binaryExpr.Right is ConstantExpression constant)
            {
                var columnName = member.Member.Name;
                var paramName = "@p0";
                var paramValue = constant.Value ?? DBNull.Value;

                var sql = $"DELETE FROM [{tableName}] WHERE [{columnName}] = {paramName}";
                var param = new SqlParameter(paramName, paramValue);

                return db.Database.ExecuteSqlRaw(sql, param);
            }

            throw new NotSupportedException("Only simple equality filters are supported.");
        }

        private static System.Data.Common.DbParameter GetDbParameter(string Name, object Value)
        {
            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                return new OracleParameter(Name, Value) as System.Data.Common.DbParameter;
            }
            return new SqlParameter(Name, Value) as System.Data.Common.DbParameter;

        }
        public static IEnumerable<T> UpdateWhereNotFast<T>(this DbSet<T> Input, Func<T, Boolean> Objects, Action<T> UpdateAction)
            where T : class
        {
            var I = Input.Where(Objects).ToList();
            I.ForEach(UpdateAction);
            return I;
        }
        private static object GetPropertyValue(object o, string Name)
        {
            return o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(x => x.Name == Name).First().GetValue(o, null);
        }
    }

}
