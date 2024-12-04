
using PropertyWebApp.Models;
using System.ComponentModel.DataAnnotations;

public class RepairCosts
{
    [Key]
    public int PaymentId { get; set; } 
    public int RepairId { get; set; } // FK na Repair
    public int RentalId { get; set; } // FK na Rental

    // Navigation properties
    public Repair Repair { get; set; } 
    public Rental Rental { get; set; } 
}
