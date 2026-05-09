namespace app.DTOs.Students;

public class StudentResponse
{
    public long Id { get; init; }
    public long? UserId { get; init; }
    public string? StudentCode { get; init; }
    public DateOnly? DateOfBirth { get; init; }
    public byte? Gender { get; init; }
    public string? Address { get; init; }
    public string? ParentName { get; init; }
    public string? ParentPhone { get; init; }
    public string? ParentEmail { get; init; }
    public byte? Source { get; init; }
    public long? AssignedStaffId { get; init; }
    public byte Status { get; init; }
    public DateTime? EnrolledAt { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
}
