using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Chronozoom.AdlibImporter.Backend.Axiell
{
    [XmlRoot("adlibXML")]
    public class AdlibFacetsRecords
    {
        [XmlElement("diagnostic")]
        public Diagnostic Diagnostic { get; set; }

        [XmlArray("recordList")]
        [XmlArrayItem("record")]
        public AdlibFacetRecord[] Records { get; set; }
    }

    public class AdlibFacetRecord
    {
        [XmlElement("term")]
        public string Term { get; set; }

        [XmlElement("count")]
        public string Count { get; set; }
    }
}
