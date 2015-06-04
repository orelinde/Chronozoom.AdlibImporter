namespace Chronozoom.AdlibImporter.Backend.Models
{
    using System.ComponentModel.DataAnnotations;
    public class GroupAction
    {
        [Required]
        public string GroupBy { get; set; }
        public string CategoryName { get; set; }
    }
}