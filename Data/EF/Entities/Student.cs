namespace app.Data.EF.Entities;

public class Student
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string? StudentCode { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    // male 1 | female 2 | other 3
    public byte? Gender { get; set; }
    public string? Address { get; set; }
    public string? ParentName { get; set; }
    public string? ParentPhone { get; set; }
    public string? ParentEmail { get; set; }
    // facebook 1 | zalo 2 | referral 3 | walk_in 4 | website 5
    public byte? Source { get; set; }
    public long? AssignedStaffId { get; set; }
    // active 1 | inactive 2 | graduated 3 | suspended 4
    public byte Status { get; set; } = 1;
    public DateTime? EnrolledAt { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
