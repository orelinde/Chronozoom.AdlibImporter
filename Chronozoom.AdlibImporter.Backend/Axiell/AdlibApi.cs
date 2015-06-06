using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Chronozoom.AdlibImporter.Backend.Axiell
{
    public class AdlibApi
    {
        public static AdlibFacetsRecords GetFacets(string url, string database, string facet)
        {
            var uri = String.Format("{0}/wwwopac.ashx?database={1}&command=facets&search=all&facet={2}&xmltype=unstructured&limit=800", url, database, facet);
            using (var client = new HttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                string result = client.GetStringAsync(new Uri(uri)).Result;
                XmlSerializer serializer = new XmlSerializer(typeof(AdlibFacetsRecords));
                StringReader reader = new StringReader(result);
                return (AdlibFacetsRecords)serializer.Deserialize(reader);

            }
        }

        public static List<AdlibRecord> GetContentItemsByFacets(string url, string database, string facet, AdlibFacetsRecords rootFacets,string titleElement)
        {
            var items = new ConcurrentBag<AdlibRecord>();

            Parallel.ForEach(rootFacets.Records, rfacet =>
            {
                using (var client = new HttpClient())
                {
                    var uri = String.Format("{0}/wwwopac.ashx?database={1}&search={2}={3}&xmltype=unstructured", url,
                        database, facet, rfacet.Term);
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    string result = client.GetStringAsync(new Uri(uri)).Result;
                    XmlDocument document = new XmlDocument();
                    document.Load(new StringReader(result));

                    var records = document.GetElementsByTagName("record");
                    foreach (XmlElement record in records)
                    {
                        var rec = new AdlibRecord();
                        rec.Priref = int.Parse(record.Attributes.GetNamedItem("priref").Value);
                        rec.Properties = record.ChildNodes;
                        rec.Title = GetTitleElement(record.ChildNodes, titleElement);
                        items.Add(rec);
                    }
                }
            });

            return items.ToList();
        }

        private static string GetTitleElement(XmlNodeList childNodes, string titleElement)
        {
            var enumerator = childNodes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = (XmlElement)enumerator.Current;
                if (current.Name == titleElement) return current.InnerText;
            }
            return null;
        }

        public class AdlibRecord
        {
            public string Title { get; set; }
            public int Priref { get; set; }
            public XmlNodeList Properties { get; set; }

        }
    }
}