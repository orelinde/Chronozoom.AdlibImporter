using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private string filename = "";
        public BatchProcessor(BatchCommand command)
        {
            this.command = command;
        }
        public void StartNewAndWriteToFile(string filepath)
        {
            var totalBatchTask = Task.Factory.StartNew(() =>
            {
                filename = String.Format("{0}App_Data\\Batch-{1}.txt", filepath, DateTime.UtcNow.Ticks);
                Timeline timeline = CreateTimeline();

                AdlibFacetsRecords adlibFacetsRecords = AdlibApi.GetFacets(command.BaseUrl, command.Database, command.Actions[0].GroupBy);
                var dictionary = AdlibApi.GetContentItemsByFacets(command.BaseUrl, command.Database, command.Actions[0].GroupBy, adlibFacetsRecords, command.Mappings.Title);
                List<ContentItem> items = StartGroupingItems(dictionary, timeline);
                timeline.BeginDate = items.Min(r => r.BeginDate);
                timeline.EndDate = items.Max(r => r.EndDate);
                File.AppendAllText(filename, timeline.ToString() + "\r\n");
                WriteChildrenToFile(items,timeline);
            });
        }

        private void WriteChildrenToFile(List<ContentItem> items,Timeline timeline)
        {
            foreach (var ci in items)
            {
                ci.ParentId = timeline.Id;
                WriteChildrenRecursive(ci);
            }
        }

        private void WriteChildrenRecursive(ContentItem contentItem)
        {
            File.AppendAllText(filename, contentItem.ToString() + "\r\n");
            if (contentItem.Children == null) return;
            foreach (var child in contentItem.Children)
            {
                WriteChildrenRecursive(child);
            }
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

        private List<ContentItem> StartGroupingItems(ConcurrentDictionary<string, List<AdlibApi.AdlibRecord>> dictionary, Timeline timeline)
        {
            List<Task> tasks = new List<Task>();
            var children = new ConcurrentBag<ContentItem>();
            //Loop through the dictionary with the first action
            for (int i = 0; i < dictionary.Count; i++)
            {
                var key = dictionary.Keys.ToList()[i];
                List<AdlibApi.AdlibRecord> values;
                dictionary.TryGetValue(key, out values);

                var i1 = i;
                Task t = Task.Factory.StartNew((() =>
                {
                    // Stack to place nodes on so list with i.e. Creators => Techniques => Material
                    var stack = new Stack<Tuple<GroupAction, List<AdlibApi.AdlibRecord>>>();

                    // Loop over al lthe actions to group the data by
                    for (var j = 0; j < command.Actions.Count(); j++)
                    {
                        var action = command.Actions[j];
                        if (j != 0)
                        {
                            List<AdlibApi.AdlibRecord> adlibRecords = GetItemsByCommandAndRemoveFromList(stack.Peek().Item2, action);
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
                    children.Add(TraverseStackAndCreateContentItems(stack));
                }));
                tasks.Add(t);
            }
            Task.WaitAll(tasks.ToArray());
            foreach (ContentItem ci in children)
            {
                ci.ParentId = timeline.Id;
            }
            return children.ToList();
        }

        private ContentItem TraverseStackAndCreateContentItems(Stack<Tuple<GroupAction, List<AdlibApi.AdlibRecord>>> stack)
        {
            ContentItem deeperItem = null;
            while (stack.Count > 0)
            {
                var item = stack.Pop();
                var category = CreateParentItem(item.Item1.CategoryName);
                category.Children = CreateChildren(item.Item2, category.Id);

                if (deeperItem != null)
                {
                    category.Children.Add(deeperItem);
                    deeperItem.ParentId = category.Id;
                }
                if (category.Children.Any())
                {
                    category.BeginDate = category.Children.Min(r => r.BeginDate);
                    category.EndDate = category.Children.Max(r => r.EndDate);
                }
                deeperItem = category;
            }
            //ContentItem child = null;
            //while (stack.Count > 0)
            //{
            //    var item = stack.Pop();
            //    ContentItem parent = CreateParentItem(item.Item1.CategoryName);
            //    List<ContentItem> children = CreateChildren(item.Item2, parent.Id);
            //    parent.Children = children;
            //    if (child != null)
            //    {
            //        parent.Children.Add(child);
            //        parent.ParentId = child.Id;
            //    }
            //    if (parent.Children.Any())
            //    {
            //        parent.BeginDate = parent.Children.Min(r => r.BeginDate);
            //        parent.EndDate = parent.Children.Max(r => r.EndDate);
            //    }
            //    child = parent;
            //}
            return deeperItem;
        }

        private ContentItem CreateParentItem(string categoryName)
        {
            return new ContentItem()
            {
                Id = GetNextId(),
                Title = categoryName,
                HasChildren = true,
            };
        }

        private List<ContentItem> CreateChildren(List<AdlibApi.AdlibRecord> item2, long parentid)
        {
            ConcurrentBag<ContentItem> items = new ConcurrentBag<ContentItem>();
            Parallel.ForEach(item2, item =>
            {
                var ci = new ContentItem();
                ci.Title = GetNodeFromAdlibRecord(item, command.Mappings.Title);
                ci.Description = GetNodeFromAdlibRecord(item, command.Mappings.Description);
                ci.BeginDate = GetDecimal(GetNodeFromAdlibRecord(item, command.Mappings.Begindate));
                ci.EndDate = GetDecimal(GetNodeFromAdlibRecord(item, command.Mappings.Enddate));
                ci.PictureURLs = GetArray(item, command.Mappings.Images, command.ImagesLocation);
                ci.SourceURL = String.Format("{0}/wwwopac.ashx?database={1}&search=priref={2}&xmltype=unstructured",
                    command.BaseUrl, command.Database, item.Priref);
                ci.Id = GetNextId();
                ci.ParentId = parentid;
                ci.Children = new List<ContentItem>();
                if ((ci.BeginDate == -1 && ci.EndDate == -1) || (ci.BeginDate > ci.EndDate)) return;
                items.Add(ci);
            });
            return items.ToList();
        }

        private long GetNextId()
        {
            return Interlocked.Increment(ref IdCounter);
        }

        private List<string> GetArray(AdlibApi.AdlibRecord nodeFromAdlibRecord, string getNodeFromAdlibRecord, string url)
        {
            var list = new List<string>();
            IEnumerable<XmlNode> xmlNodes = nodeFromAdlibRecord.GetNodes().Where(r => r.Name == getNodeFromAdlibRecord);
            foreach (var node in xmlNodes)
            {
                var i = node.InnerText.ToLower();
                if ((i.Contains("jpg") || i.Contains("png") || i.Contains("jpeg") || i.Contains("tiff") || i.Contains("tif")))
                {
                    list.Add(url + node.InnerText);
                }
            }
            return list;
        }

        private decimal GetDecimal(string nodeToParse)
        {
            decimal result;
            var parsed = Decimal.TryParse(nodeToParse, out result);
            return parsed ? result : -1;
        }

        private string GetNodeFromAdlibRecord(AdlibApi.AdlibRecord record, string mapping)
        {
            var node = record.GetNodes().FirstOrDefault(r => r.Name == mapping);
            return node == null ? "" : node.InnerText;
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
        private List<AdlibApi.AdlibRecord> GetItemsByCommandAndRemoveFromList(List<AdlibApi.AdlibRecord> parentList, GroupAction action)
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