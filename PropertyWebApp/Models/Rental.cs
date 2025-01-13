using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyWebApp.Models
{
    public class Rental
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RentalId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int PropertyId { get; set; } // FK to Property
        //public string PropertyOwnerId { get; set; } // FK to User
        public string TenantId { get; set; } // FK to User

        // Navigation properties
        public Property Property { get; set; }
        public ICollection<Issue> Issues { get; set; }
        public ICollection<MonthlyPayment> Payments { get; set; } // Added navigation to MonthlyPayment
    }
}
