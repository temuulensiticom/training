using System.ComponentModel.DataAnnotations;
using loginform.Models;

namespace loginform.Pages.Admin.Users;

public sealed class UserInput
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
    public string Role { get; set; } = UserRoles.Standard;

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6)]
    [Display(Name = "Password")]
    public string? Password { get; set; }
}
