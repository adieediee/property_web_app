using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class IssueStatus
    {
        [Key]
        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public string Color { get; set; }

        // Navigačná vlastnosť
        public ICollection<Issue> Issues { get; set; }
    }
}
