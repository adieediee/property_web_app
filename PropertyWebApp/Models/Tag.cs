using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Tag
    {
        [Key]
        public int TagId { get; set; }
        public string TagName { get; set; }

        // Navigation properties
        public ICollection<TaggedIssue> TaggedIssues { get; set; }
    }
}
