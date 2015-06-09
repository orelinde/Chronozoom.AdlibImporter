using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Chronozoom.AdlibImporter.Backend.Models.ChronoZoom
{
    public class Timeline
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal BeginDate { get; set; }
        public decimal EndDate { get; set; }
        public bool IsPublic { get; set; }
        public ContentItem RootContentItem { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(JsonHelper.BeginObject());
            builder.Append(JsonHelper.PropertyToJson("Id", Id));
            builder.Append(JsonHelper.PropertyToJson("Title", Title));
            builder.Append(JsonHelper.PropertyToJson("Description", Description));
            builder.Append(JsonHelper.PropertyToJson("BeginDate", BeginDate));
            builder.Append(JsonHelper.PropertyToJson("EndDate", EndDate));
            builder.Append(JsonHelper.PropertyToJson("IsPublic", IsPublic,true));
            builder.Append(JsonHelper.EndObject());
            return builder.ToString();
        }
    }
}