namespace app.Data.EF.Entities;

public class ClassStudent
{
    public long Id { get; set; }
    public long ClassId { get; set; }
    public long StudentId { get; set; }
    public long EnrollmentId { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public byte Status { get; set; } = 1;
    public long? AddedBy { get; set; }
    public long? RemovedBy { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
