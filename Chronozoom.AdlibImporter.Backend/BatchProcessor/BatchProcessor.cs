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
            //Loop through the dictionary with the first action
            for (int i = 0; i < dictionary.Count; i++)
            {
                var key = dictionary.Keys.ToList()[i];
                List<AdlibApi.AdlibRecord> values;
                dictionary.TryGetValue(key, out values);

                var children = new ConcurrentBag<ContentItem>();
                Task t = Task.Factory.StartNew((() =>
                {
                    // Stack to plac nodes on so list with i.e. Creators => Techniques => Material
                    var stack = new Stack<Tuple<GroupAction, List<AdlibApi.AdlibRecord>>>();
                   
                    // Loop over al lthe actions to group the data by
                    for (var j = 0; j < command.Actions.Count(); j++)
                    {
                        var action = command.Actions[j];
                        Debug.WriteLine(action.GroupBy);
                        if (j != 0)
                        {
                            List<AdlibApi.AdlibRecord> adlibRecords = GetItemsByCommandAndRemoveFromList(stack.Peek().Item2,action);
                            var tuple = new Tuple<GroupAction, List<AdlibApi.AdlibRecord>>(command.Actions[j], adlibRecords);
                            stack.Push(tuple);
                        }
                            // j == 0 create stack and place the first values in it
                        else
                        {
                            var tuple = new Tuple<GroupAction, List<AdlibApi.AdlibRecord>>(command.Actions[j], values);
                            stack.Push(tuple);
                        }
                    }
                    // Traverse stack and create parent items
                }));
                var debug = true;
            }
        }

        /// <summary>
        /// Takes all the items from the parent by action and removes them from the parent. 
        /// This means that all items are available on that node, and from there on we take the 
        /// items that we need to the next node. so if we want to create a timeline with grouping Creators => Technique => Material
        /// then we start taking all the items from creators which has a technique, and returns this list
        /// </summary>
        /// <param name="parentList">The list with items i.e. Creators</param>
        /// <param name="action">The action what it should take from the list i.e Technique</param>
        /// <returns></returns>
        private List<AdlibApi.AdlibRecord> GetItemsByCommandAndRemoveFromList(List<AdlibApi.AdlibRecord> parentList,GroupAction action)
        {
            var items = new List<AdlibApi.AdlibRecord>();
            foreach (var record in parentList)
            {
                items.AddRange(from node in record.GetNodes() where node.Name == action.GroupBy select record);
            }
            foreach (var record in items)
            {
                parentList.Remove(record);
            }
            return items;
        }
    }
}