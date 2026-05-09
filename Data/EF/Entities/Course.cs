namespace app.Data.EF.Entities;

public class Course
{
    public long Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public long? CategoryId { get; set; }
    public string? Level { get; set; }
    public int? TotalSessions { get; set; }
    public int? DurationMinutes { get; set; }
    public decimal? Price { get; set; }
    public string Currency { get; set; } = "VND";
    public string? ThumbnailUrl { get; set; }
    public byte Status { get; set; } = 1;
    public long? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
