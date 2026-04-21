namespace app.Application.DTOs;

public class UserDto
{
    public ulong Id { get; set; }
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phonenumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
