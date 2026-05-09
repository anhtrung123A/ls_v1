namespace app.DTOs.Classes;

public class ClassRequest
{
    public long CourseId { get; set; }
    public string? ClassCode { get; set; }
    public string? Name { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int? MaxStudents { get; set; }
    public int? CurrentCount { get; set; }
    public byte? Type { get; set; }
    public byte? Status { get; set; }
    public long? CreatedBy { get; set; }
}
