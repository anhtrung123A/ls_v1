namespace app.DTOs.Users;

public class UserProfileResponse
{
    public long Id { get; init; }
    public string? FullName { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public byte? Role { get; init; }
    public string? AvatarUrl { get; init; }
    public bool IsActive { get; init; }
}
