namespace app.Application.DTOs;

public class CreateBranchUserDto
{
    public string? Firstname { get; set; }
    public string Lastname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phonenumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public ulong BranchId { get; set; }
    public ulong RoleId { get; set; }
}
