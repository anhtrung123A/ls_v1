namespace app.DTOs.Analytics;

public class AttendanceByStudentItemResponse
{
    public long StudentId { get; init; }
    public string? StudentCode { get; init; }
    public string? StudentName { get; init; }
    public long ClassCount { get; init; }
    public long TotalRecords { get; init; }
    public long PresentCount { get; init; }
    public long AbsentCount { get; init; }
    public decimal AttendanceRate { get; init; }
    public decimal AbsentRate { get; init; }
}
