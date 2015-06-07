using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Chronozoom.AdlibImporter.Backend.Axiell;
using Chronozoom.AdlibImporter.Backend.Models;
using Chronozoom.AdlibImporter.Backend.Models.ChronoZoom;

namespace Chronozoom.AdlibImporter.Backend.BatchProcessor
{
    public class BatchProcessor
    {
        public static long IdCounter = 0;
        private BatchCommand command;

        public BatchProcessor(BatchCommand command)
        {
            this.command = command;
        }
        public void StartNew()
        {
            var totalBatchTask = Task.Factory.StartNew(() =>
            {
                Timeline timeline = CreateTimeline();
                timeline.RootContentItem = CreateRootNode();

                AdlibFacetsRecords adlibFacetsRecords = AdlibApi.GetFacets(command.BaseUrl, command.Database, command.Actions[0].GroupBy);
                var dictionary = AdlibApi.GetContentItemsByFacets(command.BaseUrl, command.Database, command.Actions[0].GroupBy, adlibFacetsRecords, command.Mappings.Title);
                StartGroupingItems(dictionary, timeline);
            });
        }

        private ContentItem CreateRootNode()
        {
            long id = Interlocked.Increment(ref IdCounter);
            return new ContentItem()
            {
                Title = command.Actions[0].CategoryName,
                HasChildren = true,
                Id = id
            };

        }

        private Timeline CreateTimeline()
        {
            Timeline timeline = new Timeline()
            {
                Id = Interlocked.Increment(ref IdCounter),
                Title = command.Title,
                Description = command.Description,
                IsPublic = true,
                RootContentItem = new ContentItem()
            };
            return timeline;
        }

        private void StartGroupingItems(ConcurrentDictionary<string, List<AdlibApi.AdlibRecord>> dictionary, Timeline timeline)
        {
            for (int i = 0; i < dictionary.Count; i++)
            {
                var key = dictionary.Keys.ToList()[i];
                List<AdlibApi.AdlibRecord> values;
                dictionary.TryGetValue(key, out values);

                var children = new ConcurrentBag<ContentItem>();
                Task t = Task.Factory.StartNew((() =>
                {
                    // <GroupAction, List<ContentItems>
                    var stack = new Stack<Tuple<GroupAction, List<AdlibApi.AdlibRecord>>>();
                    // Skip the first action because that is the root with items. 
                    for (var j = 0; j < command.Actions.Count(); j++)
                    {
                        var action = command.Actions[j];
                        Debug.WriteLine(action.GroupBy);
                        if (j != 0)
                        {
                            List<AdlibApi.AdlibRecord> adlibRecords = GetItemsByCommandAndRemoveFromList(stack.Peek().Item2,action);
                            var tuple = new Tuple<GroupAction, List<AdlibApi.AdlibRecord>>(command.Actions[j], adlibRecords);
                            stack.Push(tuple);
                            Debug.WriteLine("j!=0");
                        }
                        else
                        {
                            var tuple = new Tuple<GroupAction, List<AdlibApi.AdlibRecord>>(command.Actions[j], values);
                            stack.Push(tuple);
                            Debug.WriteLine("J==0");
                        }
                    }
                    // Traverse stack and create parent items
                }));
                var debug = true;
            }
        }

        private List<AdlibApi.AdlibRecord> GetItemsByCommandAndRemoveFromList(List<AdlibApi.AdlibRecord> item2,GroupAction action)
        {
            var items = new List<AdlibApi.AdlibRecord>();
            foreach (var record in item2)
            {
                items.AddRange(from node in record.GetNodes() where node.Name == action.GroupBy select record);
            }
            foreach (var record in items)
            {
                item2.Remove(record);
            }
            return items;
        }
    }
}