using Microsoft.AspNetCore.Identity;
using static PropertyWebApp.Components.Pages.RegisterScreen;
using System.ComponentModel.DataAnnotations;

namespace PropertyWebApp.Models.Services
{
    public class RegisterValidator
    {
        private readonly UserManager<IdentityUser> _userManager;

        public RegisterValidator(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<string>> ValidateRegisterModel(RegisterModel registerModel)
        {
            var errors = new List<string>();

            
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(registerModel);
            if (!Validator.TryValidateObject(registerModel, validationContext, validationResults, true))
            {
                errors.AddRange(validationResults.Select(v => v.ErrorMessage));
            }

            
            var existingUser = await _userManager.FindByEmailAsync(registerModel.Email);
            if (existingUser != null)
            {
                errors.Add("Používateľ s týmto e-mailom už existuje.");
            }

           
            if (!IsPasswordStrong(registerModel.Password))
            {
                errors.Add("Heslo musí obsahovať aspoň 8 znakov, jedno veľké písmeno, jedno malé písmeno, jedno číslo a jeden špeciálny znak.");
            }

           

            return errors;
        }

        private bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
    }

}
