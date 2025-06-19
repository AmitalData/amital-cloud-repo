using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Xml;

namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public static class ProxyHelper
    {
        public static Func<string, string, int, bool> SecurityUtilityCheckFeature { get; set; }


        public static string JsonConvertSerialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static XmlDocument DeserializeXmlNode(string jsonString)
        {
            return JsonConvert.DeserializeXmlNode(jsonString, "CourierHawb");
        }
        public static T JsonConvertDeserializeTyped<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
        public static object JsonConvertDeserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject(jsonString);
        }
        public static List<string> GetRequiredFieldInArrayJson(string onedimensionalarrayJson, List<string> mustFields)
        {
            var emptyFields = new List<string>();
            var obj = JToken.Parse(onedimensionalarrayJson);
            foreach (var field in mustFields)
            {
                string val = (string)obj[field] ?? "";
                if (string.IsNullOrWhiteSpace(val))
                {
                    emptyFields.Add(field);
                }

            }
            return emptyFields;


        }

        public static bool ArrayJsonAreEqual(string onedimensionalarrayJson1, string onedimensionalarrayJson2, List<string> dontCheckFields)
        {
            var emptyFields = new List<string>();
            var obj1 = JsonConvert.DeserializeObject<JObject>(onedimensionalarrayJson1);
            var obj2 = JToken.Parse(onedimensionalarrayJson2);
            foreach (KeyValuePair<string, JToken> parsedObject in obj1)
            {

                {
                    if (dontCheckFields.Contains(parsedObject.Key))
                    {
                        continue;
                    }
                    string val1 = (string)parsedObject.Value ?? "";
                    string val2 = (string)obj2[parsedObject.Key] ?? "";
                    if (val1 != val2)
                    {
                        return false;
                    }
                }



            }
            return true;


        }
    }

}
