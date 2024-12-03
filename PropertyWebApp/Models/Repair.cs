
using PropertyWebApp.Models;
using System.ComponentModel.DataAnnotations;

public class Repair
{
    [Key]
    public int RepairId { get; set; }
    public int IssueId { get; set; } // Foreign Key
    public decimal RepairCost { get; set; }
    public DateTime DateOfRepair { get; set; }

    // Navigation property
    public Issue Issue { get; set; }
}
