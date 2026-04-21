namespace app.Application.DTOs;

public class CreateUserDto
{
    public string Fullname { get; set; } = string.Empty;
    public string? Firstname { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phonenumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}
