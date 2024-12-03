
using PropertyWebApp.Models;
using System.ComponentModel.DataAnnotations;

public class RepairCosts
{
    [Key]
    public int PaymentId { get; set; } // Foreign Key
    public int RepairId { get; set; } // Foreign Key
    public int RentalId { get; set; } // Foreign Key

    // Navigation properties
    public Repair Repair { get; set; } 
    public Rental Rental { get; set; } 
}
