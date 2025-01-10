using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using PropertyWebApp.Models.Services;

namespace PropertyWebApp.Components.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserStateService _userStateService;

        public LogoutModel(SignInManager<IdentityUser> signInManager, UserStateService userStateService)
        {
            _signInManager = signInManager;
            _userStateService = userStateService;
        }

        public void OnGet()
        {
            // The GET request simply renders the confirmation page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync(); // Perform the logout
            _userStateService.Clear(); // Clear the user state

            return RedirectToPage("/Login"); // Redirect to the Login page
        }
    }
}
