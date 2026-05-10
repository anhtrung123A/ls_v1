namespace app.DTOs.Analytics;

public class AttendanceByClassItemResponse
{
    public long ClassId { get; init; }
    public string ClassCode { get; init; } = string.Empty;
    public string? ClassName { get; init; }
    public long TotalRecords { get; init; }
    public long PresentCount { get; init; }
    public long AbsentCount { get; init; }
    public decimal AttendanceRate { get; init; }
    public decimal AbsentRate { get; init; }
}
