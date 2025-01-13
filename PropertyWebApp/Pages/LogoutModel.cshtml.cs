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
            //TODO treba ju vobec v buducnsti pouzivat?
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync(); 
            _userStateService.Clear(); 

            return RedirectToPage("/Login"); 
        }
    }
}
