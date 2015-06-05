using System;
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
            var uri = String.Format("{0}/wwwopac.ashx?database={1}&command=facets&search=all&facet={2}&xmltype=unstructured&limit=1", url, database, facet);
            using (var client = new HttpClient())
            {
                client.Timeout = Timeout.InfiniteTimeSpan;
                string result = client.GetStringAsync(new Uri(uri)).Result;
                XmlSerializer serializer = new XmlSerializer(typeof(AdlibFacetsRecords));
                StringReader reader = new StringReader(result);
                return (AdlibFacetsRecords)serializer.Deserialize(reader);
                
            }
        }

        public static List<string> GetContentItemsByFacets(string url, string database, string facet,AdlibFacetsRecords rootFacets)
        {
            using (var client = new HttpClient())
            {
                Parallel.ForEach(rootFacets.Records, rfacet =>
                {
                    var uri = String.Format("{0}/wwwopac.ashx?database={1}&search={2}={3}&xmltype=unstructured", url, database,facet, rfacet.Term);
                    client.Timeout = Timeout.InfiniteTimeSpan;
                    string result = client.GetStringAsync(new Uri(url)).Result;
                    XmlDocument document = new XmlDocument();
                    document.Load(new StringReader(result));

                    var records = document.GetElementsByTagName("record");
                    foreach (var record in records)
                    {

                    }
                });
            }
            return null;
        }
    }
}