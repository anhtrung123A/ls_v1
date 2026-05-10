namespace app.DTOs.Analytics;

public class AttendanceRiskStudentItemResponse
{
    public long StudentId { get; init; }
    public string? StudentCode { get; init; }
    public string? StudentName { get; init; }
    public long ClassId { get; init; }
    public string? ClassName { get; init; }
    public long TotalRecords { get; init; }
    public long PresentCount { get; init; }
    public long AbsentCount { get; init; }
    public decimal AttendanceRate { get; init; }
    public decimal AbsentRate { get; init; }
    public DateOnly? LastAbsentDate { get; init; }
    public string RiskLevel { get; init; } = "medium";
}
