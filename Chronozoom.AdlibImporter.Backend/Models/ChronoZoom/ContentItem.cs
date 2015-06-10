using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Chronozoom.AdlibImporter.Backend.Models.ChronoZoom
{
    public class ContentItem
    {
        public long Id { get; set; }
        public decimal BeginDate { get; set; }
        public decimal EndDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool HasChildren { get; set; }
        public List<String> PictureURLs { get; set; }
        public long ParentId { get; set; }
        public string SourceURL { get; set; }
        public string SourceRef { get; set; }
        public List<ContentItem> Children { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(JsonHelper.BeginObject());
            builder.Append(JsonHelper.PropertyToJson("Id", Id));
            builder.Append(JsonHelper.PropertyToJson("Title", Title));
            builder.Append(JsonHelper.PropertyToJson("Description", Description));
            builder.Append(JsonHelper.PropertyToJson("BeginDate", BeginDate));
            builder.Append(JsonHelper.PropertyToJson("EndDate", EndDate));
            builder.Append(JsonHelper.PropertyToJson("HasChildren", HasChildren));
            builder.Append(JsonHelper.PropertyToJson("ParentId", ParentId));
            builder.Append(JsonHelper.PropertyToJson("SourceURL", SourceURL));
            builder.Append(JsonHelper.ArrayToJson("PictureURLs", PictureURLs,false));
            builder.Append(JsonHelper.PropertyToJson("SourceRef", SourceRef,true));
            builder.Append(JsonHelper.EndObject());
            return builder.ToString();
        }
    }
}