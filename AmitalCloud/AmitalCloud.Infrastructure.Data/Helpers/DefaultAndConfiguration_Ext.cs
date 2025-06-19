using AmitalCloud.Infrastructure.Model.EntityClasses;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class DefaultAndConfiguration_Ext : DefaultAndConfiguration
    {
        public DefaultAndConfiguration_Ext(DefaultAndConfiguration a)
        {
            Id = a.Id;
            Tenant = a.Tenant;
            CreateDate = a.CreateDate;
            SearchFields = a.SearchFields;
            Is_Active = a.Is_Active;
            StoreInCache = a.StoreInCache;
            SetKey = a.SetKey;
            AdditionalKey = a.AdditionalKey;
            SortOrder = a.SortOrder;
            SetValueType1 = a.SetValueType1;
            Value1 = a.Value1;
            SetValueType2 = a.SetValueType2;
            Value2 = a.Value2;
            AllowInheritance = a.AllowInheritance;
        }

        public object ObjVal1
        {
            get
            {
                return Convert(Value1, SetValueType1);
            }
        }

        public object ObjVal2
        {
            get
            {
                return Convert(Value2, SetValueType2);
            }
        }

        private object Convert(string value, string typeString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(typeString))
                    return null;
                
                Type type = Type.GetType(typeString) ?? AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetType(typeString) != null)?.GetType(typeString);
                if (type == null)
                    throw new Exception("Type not found: " + typeString);

                if (type == typeof(string))
                    return value;
                if (type == typeof(int))
                    return int.Parse(value);
                if (type == typeof(long))
                    return long.Parse(value);
                if (type == typeof(double))
                    return double.Parse(value);
                if (type == typeof(bool))
                    return bool.Parse(value);
                if (type == typeof(DateTime))
                    return DateTime.Parse(value);
                if (type == typeof(DateTimeOffset))
                    return DateTimeOffset.Parse(value);
                if (type == typeof(string))
                    return value;
                if (type == typeof(string[]))
                    return value.Split(',');
                if (type == typeof(int[]))
                    return Array.ConvertAll(value.Split(','), int.Parse);
                if (type == typeof(long[]))
                    return Array.ConvertAll(value.Split(','), long.Parse);
                if (type == typeof(double[]))
                    return Array.ConvertAll(value.Split(','), double.Parse);
                if (type == typeof(bool[]))
                    return Array.ConvertAll(value.Split(','), bool.Parse);
                if (type == typeof(DateTime[]))
                    return Array.ConvertAll(value.Split(','), DateTime.Parse);
                if (type == typeof(Guid[]))
                    return Array.ConvertAll(value.Split(','), Guid.Parse);
                if (type == typeof(DateTimeOffset[]))
                    return Array.ConvertAll(value.Split(','), DateTimeOffset.Parse);

                object deserializedObject = JsonConvert.DeserializeObject(value, type);

                return deserializedObject is Array array ? array : deserializedObject;
            }
            catch (Exception e)
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteFatal(e, "An error occurred while deserializing value: " + value + ", type: " + typeString);
                return null;
            }
        }
    }
}
