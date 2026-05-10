namespace app.DTOs.Analytics;

public class ClassFillRateItemResponse
{
    public long ClassId { get; init; }
    public string ClassCode { get; init; } = string.Empty;
    public string? ClassName { get; init; }
    public string? CourseName { get; init; }
    public int MaxStudents { get; init; }
    public int CurrentCount { get; init; }
    public int AvailableSlots { get; init; }
    public decimal FillRate { get; init; }
    public string Status { get; init; } = "upcoming";
}
