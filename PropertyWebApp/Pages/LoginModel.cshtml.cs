using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PropertyWebApp.Models.Services;


namespace PropertyWebApp.Components.Pages
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserStateService _userStateService;

        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, UserStateService userStateService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStateService = userStateService;
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
                Console.WriteLine($"Prihlásený používateľ: {user?.UserName ?? "Neznámy"}");
                var role = await _userManager.GetRolesAsync(user);
                 await _userStateService.SetUserInfo(user?.UserName ?? "Neznámy",user.Id);
                await _userStateService.SetRole(role[0]);

                return Redirect("/test"); // Presmerovanie po úspešnom prihlásení
            }

            ErrorMessage = result.IsLockedOut
                ? "Účet je zamknutý."
                : "Prihlásenie zlyhalo.";
            return Page();
        }
    }
}
