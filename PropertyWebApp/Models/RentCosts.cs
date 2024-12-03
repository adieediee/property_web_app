using PropertyWebApp.Models;
using System.ComponentModel.DataAnnotations;
public class RentCosts
{
    [Key]
    public int RentCostId { get; set; }
    public decimal RentAmount { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}
