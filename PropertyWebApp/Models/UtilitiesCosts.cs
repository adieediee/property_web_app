
using System.ComponentModel.DataAnnotations;

public class UtilitiesCosts
{
    [Key]
    public int UtilitiesCostId { get; set; }
    public decimal UtilitiesAmount { get; set; }
    public DateTime EffectiveDate { get; set; }
    public DateTime? EndDate { get; set; }
}
