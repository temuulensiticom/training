using System.ComponentModel.DataAnnotations;
using loginform.Data;
using loginform.Models;
using loginform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;

namespace loginform.Pages;

[AllowAnonymous]
public sealed class SignupModel(IUserRepository userRepository, PasswordHashService passwordHashService) : PageModel
{
    [BindProperty]
    public SignupInput Input { get; set; } = new();

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Page();
        }

        return User.IsInRole(UserRoles.Admin)
            ? RedirectToPage("/Admin/Users/Index")
            : RedirectToPage("/Welcome");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new AppUser
        {
            Username = Input.Username.Trim(),
            PasswordHash = passwordHashService.HashPassword(Input.Password),
            FirstName = Input.FirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(Input.LastName) ? null : Input.LastName.Trim(),
            Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim(),
            Role = UserRoles.Standard,
            IsActive = true
        };

        try
        {
            await userRepository.CreateAsync(user);
        }
        catch (MySqlException)
        {
            ModelState.AddModelError("Input.Username", "This username may already exist, or MySQL is not available.");
            return Page();
        }

        TempData["SignupMessage"] = "Account created. You can log in now.";
        return RedirectToPage("/Index");
    }

    public sealed class SignupInput
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Last name")]
        public string? LastName { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
