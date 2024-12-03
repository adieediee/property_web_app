using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class PropertyType
    {
        [Key]
        public int TypeId { get; set; }
        public string TypeName { get; set; }

        // Navigation properties
        public ICollection<Property> Properties { get; set; }
    }
}
