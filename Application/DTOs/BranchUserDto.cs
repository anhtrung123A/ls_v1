namespace app.Application.DTOs;

public class BranchUserDto
{
    public ulong Id { get; set; }
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phonenumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public ulong BranchId { get; set; }
    public int Status { get; set; }
    public string? AvatarUrl { get; set; }
}