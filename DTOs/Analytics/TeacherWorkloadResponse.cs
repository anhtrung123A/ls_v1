namespace app.DTOs.Analytics;

public class TeacherWorkloadResponse
{
    public required IReadOnlyList<TeacherWorkloadItemResponse> Items { get; init; }
}
