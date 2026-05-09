namespace app.Data.EF.Entities;

public class User
{
    public long Id { get; set; }

    public string? FullName { get; set; }

    public required string Email { get; set; }

    public string? Phone { get; set; }

    // admin = 1, staff = 2, teacher = 3, student = 4
    public byte? Role { get; set; }

    public string? AvatarUrl { get; set; }

    public string? PasswordHash { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
