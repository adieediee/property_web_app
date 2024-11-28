using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class IssueImage
    {
        [Key]
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
        public int IssueId { get; set; } // FK na Issue

        // Navigačná vlastnosť
        public Issue Issue { get; set; }
    }
}
