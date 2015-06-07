using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}