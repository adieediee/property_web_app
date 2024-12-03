using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class PropertyImage
    {
        [Key]
        public int ImageId { get; set; }
        public int PropertyId { get; set; } // FK to Property
        public string ImagePath { get; set; }

        // Navigation properties
        public Property Property { get; set; }
    }
}
