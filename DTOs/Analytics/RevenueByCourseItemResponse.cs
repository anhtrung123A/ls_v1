namespace app.DTOs.Analytics;

public class RevenueByCourseItemResponse
{
    public long CourseId { get; init; }
    public string CourseName { get; init; } = string.Empty;
    public long InvoiceCount { get; init; }
    public long StudentCount { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal PaidRevenue { get; init; }
    public decimal OutstandingRevenue { get; init; }
}
