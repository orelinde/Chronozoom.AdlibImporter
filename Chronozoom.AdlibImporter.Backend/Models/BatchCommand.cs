namespace Chronozoom.AdlibImporter.Backend.Models
{
    using System.ComponentModel.DataAnnotations;

    public class BatchCommand
    {
        [Required]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "No Mappings found")]
        public XmlMappings Mappings { get; set; }

        [Required(ErrorMessage = "No Actions found")]
        [MinLength(length:1,ErrorMessage = "At least one action must be present")]
        public GroupAction[] Actions { get; set; }
    }
}