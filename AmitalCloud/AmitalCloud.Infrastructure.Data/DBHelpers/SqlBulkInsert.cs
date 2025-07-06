using AmitalCloud.Infrastructure.Data.Helpers;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AmitalCloud.Infrastructure.Data.DBHelpers
{
    public class SqlBulkInsert
    {
        public static void BulkInsert<T>(string tableName, IList<T> list,int contextTenant)
        {
            if (list == null || list.Count() == 0)
                return;
            if (tableName == "TextCodes")
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    var property = item?.GetType().GetProperty("LocalDefaultText");
                    if (property != null && property.PropertyType == typeof(string))
                    {
                        var base64Value = property.GetValue(item) as string;
                        var convertedValue = TryConvertFromBase64(base64Value ?? string.Empty);
                        property.SetValue(item, convertedValue);
                    }
                }
            }
           
                RunSqlInsert(tableName, list, contextTenant);
        }

        private static void RunSqlInsert<T>(string tableName, IList<T> list, int contextTenant)
        {

            PropertyDescriptor[] entityProperties = GetEntitySystemProperties<T>();
            string strConnString = TenantServerConfigration.GetDbConnection(contextTenant);
            StringBuilder sqlStringBuilder = new StringBuilder();
            string insertCommand = "insert into " + BuildInsertCommandColumnsString(tableName, entityProperties) + " values";
            sqlStringBuilder.AppendLine(insertCommand);
            int allRowsCount = list.Count();
            int bulkInsertedRowsCount = 0;
            int insertedRowsCount = 0;
            using (SqlConnection connection = new SqlConnection(strConnString))
            {
                connection.Open();
                foreach (var item in list)
                {
                    string valuesString = BuildValuesSqlString(entityProperties, item) + "),";
                    sqlStringBuilder.AppendLine(valuesString);
                    bulkInsertedRowsCount++;
                    insertedRowsCount++;
                    if (bulkInsertedRowsCount == 100 || insertedRowsCount == allRowsCount)
                    {
                        string sqlCommandString = sqlStringBuilder.ToString();
                        sqlCommandString = sqlCommandString.Substring(0, sqlCommandString.LastIndexOf(","));
                        SqlCommand cmd = new SqlCommand(sqlCommandString, connection);
                        cmd.CommandTimeout = 1200;
                        var output = cmd.ExecuteNonQuery();

                        sqlStringBuilder = new StringBuilder();
                        sqlStringBuilder.AppendLine(insertCommand);
                        bulkInsertedRowsCount = 0;
                    }
                }
                connection.Close();
            }
        }

        
        private static PropertyDescriptor[] GetEntitySystemProperties<T>()
        {
            var props = TypeDescriptor.GetProperties(typeof(T))
                                       //Dirty hack to make sure we only have system data types 
                                       //i.e. filter out the relationships/collections
                                       .Cast<PropertyDescriptor>()
                                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                       .ToArray();
            return props;
        }

        private static string BuildInsertCommandColumnsString(string tableName, PropertyDescriptor[] entityProperties)
        {
            string insertCommand = tableName + "(";
            foreach (var propertyInfo in entityProperties)
            {
                insertCommand = AppendFieldToInsertCommand(tableName, insertCommand, propertyInfo);
            }
            insertCommand = insertCommand.TrimEnd(',') + ")";
            return insertCommand;
        }

        private static string AppendFieldToInsertCommand(string tableName, string insertCommand, PropertyDescriptor propertyInfo)
        {
            insertCommand += propertyInfo.Name + ",";
            return insertCommand;
        }

        private static string BuildValuesSqlString<T>(PropertyDescriptor[] entityProperties, T item)
        {
            string valuesString = "(";
            for (var i = 0; i < entityProperties.Length; i++)
            {
                object propValue = entityProperties[i].GetValue(item);
                if (entityProperties[i].PropertyType != typeof(int) && entityProperties[i].PropertyType != typeof(decimal)
                                       && entityProperties[i].PropertyType != typeof(float) && entityProperties[i].PropertyType != typeof(double))
                {
                    if (propValue != null)
                    {
                        if (entityProperties[i].PropertyType == typeof(bool))
                            propValue = propValue.ToString().ToLower() == "true" ? 1 : 0;
                        else
                            propValue = "N'" + propValue.ToString().Replace("'", "''") + "'";
                    }
                    else
                        propValue = "NULL";
                }

                valuesString += propValue + ",";
            }
            valuesString = valuesString.TrimEnd(',');
            return valuesString;
        }
        
        public static string TryConvertFromBase64(string input)
        {
            try
            {
                if (input == null)
                {
                    return null;
                }
                if (input.StartsWith("BS64:") || input.StartsWith("\"BS64:"))
                {

                    return ConvertFromBase64(input);


                }
                return input;

            }
            catch (FormatException)
            {
                return input;
            }
        }

        private static string ConvertFromBase64(string input)
        {
            string backUp = input;
            try
            {
                input = input.Trim('\"');
                input = input.Substring(5);//REMOVE BS64:
                byte[] data = Convert.FromBase64String(input);
                string decodedString = Encoding.UTF8.GetString(data);
                decodedString = decodedString.Trim('\"');

                return decodedString;

            }
            catch (FormatException)
            {
                return backUp;
            }

        }

    }
}
