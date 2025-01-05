
using PropertyWebApp.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyWebApp.Models
{
    public class MonthlyPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }

        [ForeignKey("Rental")]
        public int RentalId { get; set; } // Foreign Key to Rental
        public Rental Rental { get; set; } // Navigation property to Rental
        public DateTime? PaymentDate { get; set; }
        public bool isPaid { get; set; }
        public decimal RentAmount { get; set; } // Added for clarity
        public decimal UtilitiesAmount { get; set; } // Added for clarity
        public decimal TotalAmount => RentAmount + UtilitiesAmount; // Convenience property
    }

}


