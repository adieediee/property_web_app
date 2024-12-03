using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class TaggedIssue
    {
        [Key]
        public int IssueId { get; set; } // FK to Issue
        public int TagId { get; set; } // FK to Tag

        // Navigation properties
        public Issue Issue { get; set; }
        public Tag Tag { get; set; }
    }
}
