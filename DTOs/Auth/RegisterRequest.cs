using System.ComponentModel.DataAnnotations;

namespace app.DTOs.Auth;

public class RegisterRequest
{
    [MaxLength(150)]
    public string? FullName { get; set; }

    [Required]
    [MaxLength(150)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Range(1, 4)]
    public byte? Role { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
