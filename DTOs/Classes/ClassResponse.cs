namespace app.DTOs.Classes;

public class ClassResponse
{
    public long Id { get; init; }
    public long CourseId { get; init; }
    public string ClassCode { get; init; } = string.Empty;
    public string? Name { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public int MaxStudents { get; init; }
    public int CurrentCount { get; init; }
    public byte Type { get; init; }
    public byte Status { get; init; }
    public long? TeacherId { get; init; }
    public long? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
}
