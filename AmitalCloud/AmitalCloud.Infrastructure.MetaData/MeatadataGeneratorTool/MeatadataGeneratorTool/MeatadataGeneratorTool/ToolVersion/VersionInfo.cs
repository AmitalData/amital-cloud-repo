using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MeatadataGeneratorTool.ToolVersion
{
    [Serializable()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot("VersionInfo", Namespace = "", IsNullable = false)]
    public class VersionInfo
    {
        [XmlAttribute()]
        public string VersionNo { get; set; }
    }
}
