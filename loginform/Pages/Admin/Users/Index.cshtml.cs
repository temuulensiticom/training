using loginform.Data;
using loginform.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace loginform.Pages.Admin.Users;

[Authorize(Roles = UserRoles.Admin)]
public sealed class IndexModel(IUserRepository userRepository) : PageModel
{
    public IReadOnlyList<AppUser> Users { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Users = await userRepository.GetAllAsync();
    }
}
