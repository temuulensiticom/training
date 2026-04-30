using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace loginform.Pages;

[Authorize(Roles = "Standard")]
public sealed class WelcomeModel : PageModel
{
    public string FirstName { get; private set; } = string.Empty;

    public void OnGet()
    {
        FirstName = User.FindFirstValue(ClaimTypes.GivenName) ?? User.Identity?.Name ?? "User";
    }
}
