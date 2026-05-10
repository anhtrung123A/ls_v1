namespace app.DTOs.Analytics;

public class AttendanceRiskStudentsResponse
{
    public decimal Threshold { get; init; }
    public required IReadOnlyList<AttendanceRiskStudentItemResponse> Items { get; init; }
}
