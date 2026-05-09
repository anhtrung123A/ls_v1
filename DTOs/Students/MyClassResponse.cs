namespace app.DTOs.Students;

public class MyClassResponse
{
    public long ClassStudentId { get; init; }
    public long ClassId { get; init; }
    public string? ClassCode { get; init; }
    public string? ClassName { get; init; }
    public long CourseId { get; init; }
    public string? CourseName { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public byte ClassStatus { get; init; }
    public byte ClassStudentStatus { get; init; }
    public DateTime JoinedAt { get; init; }
    public DateTime? LeftAt { get; init; }
}
