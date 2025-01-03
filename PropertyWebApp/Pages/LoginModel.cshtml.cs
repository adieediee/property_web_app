using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace PropertyWebApp.Components.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public bool RememberMe { get; set; }
        public string? ErrorMessage { get; private set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email a heslo sú povinné.";
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, Password))
            {
                ErrorMessage = "Nesprávne prihlasovacie údaje.";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Password, false, false);
            if (result.Succeeded)
            {
                return Redirect("/test"); // Presmerovanie po úspešnom prihlásení
            }

            ErrorMessage = result.IsLockedOut
                ? "Účet je zamknutý."
                : "Prihlásenie zlyhalo.";
            return Page();
        }
    }
}
