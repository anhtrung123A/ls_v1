namespace app.Data.EF.Entities;

public class ClassAttendance
{
    public long Id { get; set; }
    public long ClassSessionId { get; set; }
    public long ClassStudentId { get; set; }
    public bool? IsAbsent { get; set; }
    public string? AbsentReason { get; set; }
    public long? RecordedBy { get; set; }
    public DateTime RecordedAt { get; set; }
}
