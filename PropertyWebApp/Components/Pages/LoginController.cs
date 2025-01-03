using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace PropertyWebApp.Controllers
{
    [Route("[controller]")]
    public class LogInController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public LogInController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, bool remember)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["ErrorMessage"] = "Email a heslo sú povinné.";
                return Redirect("/login");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            {
                TempData["ErrorMessage"] = "Nesprávne prihlasovacie údaje.";
                return Redirect("/login");
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, remember, false);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Úspešne ste sa prihlásili ako {user.UserName}.";
                return Redirect("/Prenajimatel");
            }

            TempData["ErrorMessage"] = result.IsLockedOut
                ? "Účet je zamknutý."
                : result.IsNotAllowed
                    ? "Prihlásenie nie je povolené."
                    : "Prihlásenie zlyhalo. Skontrolujte údaje.";

            return Redirect("/login");
        }
    }
}
