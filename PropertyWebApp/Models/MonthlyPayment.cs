
using PropertyWebApp.Models;
using System.ComponentModel.DataAnnotations;

public class MonthlyPayment
{
    [Key]
    public int PaymentId { get; set; }
    public string RentalId { get; set; } = string.Empty; // Foreign Key
    public DateTime? PaymentDate { get; set; }
    public int? UtilitiesCostId { get; set; } // Foreign Key
    public int? RentCostId { get; set; } // Foreign Key

    
}
