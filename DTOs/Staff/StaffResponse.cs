namespace app.DTOs.Staff;

public class StaffResponse
{
    public long Id { get; init; }
    public long UserId { get; init; }
    public string StaffCode { get; init; } = string.Empty;
    public byte? Department { get; init; }
    public string? Position { get; init; }
    public long? ManagerId { get; init; }
    public DateTime JoinedAt { get; init; }

    public string? FullName { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public byte? Role { get; init; }
}
