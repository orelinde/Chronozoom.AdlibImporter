using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Chronozoom.AdlibImporter.Backend.Axiell
{
    public class Diagnostic
    {
        [XmlElement("hits")]
        public string Hits { get; set; }
    }
}