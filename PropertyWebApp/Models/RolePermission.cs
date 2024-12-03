using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class RolePermission
    {
        [Key]
        public int RoleId { get; set; } // FK to Role
        public int PermissionId { get; set; } // FK to Permission

        // Navigation properties
       
    }
}
