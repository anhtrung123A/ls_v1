namespace app.DTOs.Courses;

public class CourseRequest
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public long? CategoryId { get; set; }
    public string? Level { get; set; }
    public int? TotalSessions { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; }
    public string? ThumbnailUrl { get; set; }
    public byte? Status { get; set; }
    public long? CreatedBy { get; set; }
}
