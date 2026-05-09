namespace app.DTOs.Courses;

public class CourseResponse
{
    public long Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public long? CategoryId { get; init; }
    public string? Level { get; init; }
    public int? TotalSessions { get; init; }
    public int? DurationMinutes { get; init; }
    public decimal? Price { get; init; }
    public string Currency { get; init; } = "VND";
    public string? ThumbnailUrl { get; init; }
    public byte Status { get; init; }
    public long? CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
}
