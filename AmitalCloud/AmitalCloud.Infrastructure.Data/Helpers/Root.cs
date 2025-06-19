using System.Collections.Generic;
using System.Xml.Serialization;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class Root
    {
        public List<Change> Changes { get; set; }
    }

    public class Change //property change class
    {
        [XmlAttribute]
        public string fieldName { get; set; }// field name
        [XmlAttribute]
        public string oldValue { get; set; }// old value
        [XmlAttribute]
        public string newValue { get; set; }// new value
    }
}
