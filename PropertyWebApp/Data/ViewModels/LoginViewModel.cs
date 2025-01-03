using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Data.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email je povinný.")]
        [EmailAddress(ErrorMessage = "Zadajte platnú emailovú adresu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Heslo je povinné.")]
        [MinLength(6, ErrorMessage = "Heslo musí mať aspoň 6 znakov.")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false; 
    }

}
