using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using loginform.Data;
using loginform.Models;
using loginform.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace loginform.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHashService _passwordHashService;

        public IndexModel(IUserRepository userRepository, PasswordHashService passwordHashService)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        public IActionResult OnGet()
        {
            return User.Identity?.IsAuthenticated == true ? RedirectForRole() : Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            AppUser? user;
            try
            {
                user = await _userRepository.GetByUsernameAsync(Input.Username.Trim());
            }
            catch (MySqlException)
            {
                ModelState.AddModelError(string.Empty, "Cannot connect to MySQL on localhost:3306. Check that MySQL is running and the root password is correct.");
                return Page();
            }

            if (user is null || !user.IsActive || !_passwordHashService.VerifyPassword(Input.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return user.Role == UserRoles.Admin
                ? RedirectToPage("/Admin/Users/Index")
                : RedirectToPage("/Welcome");
        }

        private IActionResult RedirectForRole()
        {
            return User.IsInRole(UserRoles.Admin)
                ? RedirectToPage("/Admin/Users/Index")
                : RedirectToPage("/Welcome");
        }

        public sealed class LoginInput
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } = string.Empty;
        }
    }
}
