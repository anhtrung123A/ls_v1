namespace app.DTOs.Classes;

public class CreateClassRequest
{
    public long CourseId { get; set; }
    public string? Name { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int? MaxStudents { get; set; }
    public int? CurrentCount { get; set; }
    public byte? Type { get; set; }
    public long? TeacherId { get; set; }
}
