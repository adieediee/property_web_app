using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models
{
    public class Permission
    {
        [Key]
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }

        
    }
}
