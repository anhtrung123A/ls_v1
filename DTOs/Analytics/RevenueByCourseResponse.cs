namespace app.DTOs.Analytics;

public class RevenueByCourseResponse
{
    public required IReadOnlyList<RevenueByCourseItemResponse> Items { get; init; }
}
