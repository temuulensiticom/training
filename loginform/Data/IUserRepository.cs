using loginform.Models;

namespace loginform.Data;

public interface IUserRepository
{
    Task EnsureCreatedAsync();

    Task<AppUser?> GetByUsernameAsync(string username);

    Task<AppUser?> GetByIdAsync(int id);

    Task<IReadOnlyList<AppUser>> GetAllAsync();

    Task CreateAsync(AppUser user);

    Task UpdateAsync(AppUser user, string? newPasswordHash);
}
