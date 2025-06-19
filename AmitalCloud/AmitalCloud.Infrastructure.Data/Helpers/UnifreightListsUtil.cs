using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml.Linq;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class UnifreightListsUtil
    {
        //<![CDATA[ ]]>
        public static string Serialize(Dictionary<string, string> ListEntry, string remarks = null)
        {
            var rem = "xml generated  FROM " + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrWhiteSpace(remarks))
            {
                rem += Environment.NewLine + remarks;
            }
            var doc = new XDocument(
new XDeclaration("1.0", "utf-8", "yes"),
new XComment(rem),
    new XElement("ArrayOfEntry",
    from c in ListEntry
    select new XElement("Entry",
        new XElement("Key", new XCData(c.Key)),
        new XElement("Value", new XCData(c.Value))
    )
    )
);
            var outPut = doc.ToString(SaveOptions.None);
            return outPut;
        }
        public static Dictionary<string, string> Deserialize(string xml)
        {
            var doc = XDocument.Parse(xml);
            var result =
                (from node in doc.Descendants("Entry")
                 select new { key = node.Element("Key").Value, value = GetElementValue(node.Element("Value")) })
                .ToDictionary(e => e.key, e => e.value);
            return result;
        }
        private static string GetElementValue(XElement xElement)
        {
            string DisableFormatting = xElement.ToString(SaveOptions.DisableFormatting);
            if (DisableFormatting.StartsWith("<" + xElement.Name + ">"))
            {
                DisableFormatting = DisableFormatting.Substring(("<" + xElement.Name + ">").Length);
            }
            if (DisableFormatting.EndsWith("</" + xElement.Name + ">"))
            {
                var lastIndex = DisableFormatting.LastIndexOf("</" + xElement.Name + ">");
                DisableFormatting = DisableFormatting.Remove(lastIndex);
            }
            if (DisableFormatting.StartsWith("<![CDATA["))
            {
                DisableFormatting = DisableFormatting.Substring(("<![CDATA[").Length);
            }
            if (DisableFormatting.EndsWith("]]>"))
            {
                var lastIndex = DisableFormatting.LastIndexOf("]]>");
                DisableFormatting = DisableFormatting.Remove(lastIndex);
            }
            return DisableFormatting;
        }
        public static string GetValue(ref Dictionary<string, string> hash_data_in, string key)
        {
            if (key == "") return ("");
            if (hash_data_in.ContainsKey(key))
            {
                return (hash_data_in[key].ToString());
            }
            key = key.ToUpper();
            if (hash_data_in.ContainsKey(key))
            {
                return (hash_data_in[key].ToString());
            }
            return ("");
        }
        public static string GetHtmlDecodeValue(ref Dictionary<string, string> hash_data_in, string key)
        {
            var result = GetValue(ref hash_data_in, key);
            if (!string.IsNullOrEmpty(result))
            {
                result = HttpUtility.HtmlDecode(result);
            }
            return result;
        }
        public static string Serialize(System.Collections.Hashtable h, string remarks = null)
        {
            Dictionary<string, string> ListEntry = new Dictionary<string, string>();
            foreach (var item in h.Keys)
            {
                ListEntry.Add(item.ToString(), h[item].ToString());
            }
            return Serialize(ListEntry, remarks);
        }
    }
}
