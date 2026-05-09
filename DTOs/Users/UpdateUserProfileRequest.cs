using System.ComponentModel.DataAnnotations;

namespace app.DTOs.Users;

public class UpdateUserProfileRequest
{
    [MaxLength(150)]
    public string? FullName { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [Url]
    public string? AvatarUrl { get; set; }
}
