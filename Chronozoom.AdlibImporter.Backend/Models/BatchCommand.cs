namespace Chronozoom.AdlibImporter.Backend.Models
{
    using System.ComponentModel.DataAnnotations;

    public class BatchCommand
    {
        /// <summary>
        /// Timeline title
        /// </summary>
        [Required]
        public string Title { get; set; }
        
        /// <summary>
        /// The description of the timeline
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// The mappings from adlibhosting database to Chronozoom format
        /// </summary>
        [Required(ErrorMessage = "No Mappings found")]
        public XmlMappings Mappings { get; set; }

        /// <summary>
        /// The actions to apply to the data, these actions are group by actions
        /// </summary>
        [Required(ErrorMessage = "No Actions found")]
        [MinLength(length:1,ErrorMessage = "At least one action must be present")]
        public GroupAction[] Actions { get; set; }

        /// <summary>
        /// Adlibhosting url
        /// </summary>
 
        public string BaseUrl { get; set; }
        
        /// <summary>
        /// Database of url ?database=
        /// </summary>
 
        public string Database { get; set; }

        public string ImagesLocation { get; set; }
    }
}