namespace app.Application.DTOs;

public class UserProfileDto
{
    public ulong Id { get; set; }
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Phonenumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}
