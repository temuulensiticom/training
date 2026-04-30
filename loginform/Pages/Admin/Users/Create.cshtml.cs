using loginform.Data;
using loginform.Models;
using loginform.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySqlConnector;

namespace loginform.Pages.Admin.Users;

[Authorize(Roles = UserRoles.Admin)]
public sealed class CreateModel(IUserRepository userRepository, PasswordHashService passwordHashService) : PageModel
{
    [BindProperty]
    public UserInput Input { get; set; } = new();

    public SelectList RoleOptions => new(UserRoles.All);

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Input.Password))
        {
            ModelState.AddModelError("Input.Password", "Password is required.");
        }

        if (!UserRoles.All.Contains(Input.Role))
        {
            ModelState.AddModelError("Input.Role", "Select a valid role.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new AppUser
        {
            Username = Input.Username.Trim(),
            PasswordHash = passwordHashService.HashPassword(Input.Password!),
            FirstName = Input.FirstName.Trim(),
            LastName = string.IsNullOrWhiteSpace(Input.LastName) ? null : Input.LastName.Trim(),
            Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim(),
            Role = Input.Role,
            IsActive = Input.IsActive
        };

        try
        {
            await userRepository.CreateAsync(user);
        }
        catch (MySqlException)
        {
            ModelState.AddModelError("Input.Username", "This username may already exist.");
            return Page();
        }

        TempData["StatusMessage"] = "User created.";
        return RedirectToPage("./Index");
    }
}
