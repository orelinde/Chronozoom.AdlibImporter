
using System.ComponentModel.DataAnnotations;

namespace Chronozoom.AdlibImporter.Backend.Models
{
    public class XmlMappings
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string Begindate { get; set; }
        
        [Required]
        public string Enddate { get; set; }
        public string Images { get; set; }

        [Required]
        public string Id { get; set; }
    }
}