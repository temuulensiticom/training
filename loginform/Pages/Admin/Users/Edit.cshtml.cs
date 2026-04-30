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
public sealed class EditModel(IUserRepository userRepository, PasswordHashService passwordHashService) : PageModel
{
    [BindProperty]
    public int Id { get; set; }

    [BindProperty]
    public UserInput Input { get; set; } = new();

    public SelectList RoleOptions => new(UserRoles.All);

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        Id = user.Id;
        Input = new UserInput
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            IsActive = user.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!UserRoles.All.Contains(Input.Role))
        {
            ModelState.AddModelError("Input.Role", "Select a valid role.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var existingUser = await userRepository.GetByIdAsync(Id);
        if (existingUser is null)
        {
            return NotFound();
        }

        existingUser.Username = Input.Username.Trim();
        existingUser.FirstName = Input.FirstName.Trim();
        existingUser.LastName = string.IsNullOrWhiteSpace(Input.LastName) ? null : Input.LastName.Trim();
        existingUser.Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim();
        existingUser.Role = Input.Role;
        existingUser.IsActive = Input.IsActive;

        var newPasswordHash = string.IsNullOrWhiteSpace(Input.Password)
            ? null
            : passwordHashService.HashPassword(Input.Password);

        try
        {
            await userRepository.UpdateAsync(existingUser, newPasswordHash);
        }
        catch (MySqlException)
        {
            ModelState.AddModelError("Input.Username", "This username may already exist.");
            return Page();
        }

        TempData["StatusMessage"] = "User updated.";
        return RedirectToPage("./Index");
    }
}
