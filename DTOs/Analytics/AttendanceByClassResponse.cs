namespace app.DTOs.Analytics;

public class AttendanceByClassResponse
{
    public required IReadOnlyList<AttendanceByClassItemResponse> Items { get; init; }
}
