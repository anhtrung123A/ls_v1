namespace app.DTOs.Analytics;

public class ClassByCourseResponse
{
    public required IReadOnlyList<ClassByCourseItemResponse> Items { get; init; }
}
