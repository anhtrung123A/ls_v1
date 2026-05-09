namespace app.DTOs.Students;

public class CreateStudentRequest
{
    public DateOnly? DateOfBirth { get; set; }
    public byte? Gender { get; set; }
    public string? Address { get; set; }
    public string? ParentName { get; set; }
    public string? ParentPhone { get; set; }
    public string? ParentEmail { get; set; }
    public byte? Source { get; set; }
    public long? AssignedStaffId { get; set; }
    public byte? Status { get; set; }
    public DateTime? EnrolledAt { get; set; }
    public string? Notes { get; set; }
}
