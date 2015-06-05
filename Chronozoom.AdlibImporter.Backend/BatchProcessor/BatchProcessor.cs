using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Chronozoom.AdlibImporter.Backend.Axiell;
using Chronozoom.AdlibImporter.Backend.Models;

namespace Chronozoom.AdlibImporter.Backend.BatchProcessor
{
    public class BatchProcessor
    {
        public static void StartNew(BatchCommand command)
        {
            var totalBatchTask = Task.Factory.StartNew(() =>
            {
                AdlibFacetsRecords adlibFacetsRecords = AdlibApi.GetFacets(command.BaseUrl,command.Database,command.Actions[0].GroupBy);
                var items = AdlibApi.GetContentItemsByFacets(command.BaseUrl, command.Database, command.Actions[0].GroupBy, adlibFacetsRecords);
                //Parallel.ForEach(command.Actions, action =>
                //{
                //    //Download the facets list with actionname i.e. Creator.role
                //    //Download all the items per facet
                //    //
                //});
            });


        }
    }
}