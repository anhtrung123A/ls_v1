namespace app.DTOs.Analytics;

public class AttendanceByStudentResponse
{
    public required IReadOnlyList<AttendanceByStudentItemResponse> Items { get; init; }
}
