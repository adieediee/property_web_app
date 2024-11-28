using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Property
    {
        [Key]
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal MonthlyRent { get; set; }
        public bool IsAvailable { get; set; }
    }
}
