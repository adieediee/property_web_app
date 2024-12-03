

using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PropertyId { get; set; } // FK to Property
        public int PropertyOwnerId { get; set; } // FK to User
        public int TenantId { get; set; } // FK to User
         //navigation properties

        public Property Property { get; set; }
        public ICollection<Issue> Issues { get; set; }

    }
}
