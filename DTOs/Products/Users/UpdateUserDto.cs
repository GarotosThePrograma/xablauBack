using System.ComponentModel.DataAnnotations;

namespace xablau.DTOs.Users;

public class UpdateUserDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Password { get; set; }

    [Required]
    public string Role { get; set; } = "User";
}
