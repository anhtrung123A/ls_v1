namespace app.DTOs.ClassAttendances;

public class ClassAttendanceRequest
{
    public long ClassSessionId { get; set; }
    public long ClassStudentId { get; set; }
    public bool? IsAbsent { get; set; }
    public string? AbsentReason { get; set; }
}
