using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Tenant
    {
        [Key]
        public int TenantId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        // Navigačné vlastnosti
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
