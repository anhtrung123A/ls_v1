namespace app.Domain.Entities;

public class User
{
    public ulong Id { get; set; }
    public string Fullname { get; set; } = string.Empty;
    public string? Firstname { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Phonenumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
