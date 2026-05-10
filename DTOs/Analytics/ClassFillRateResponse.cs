namespace app.DTOs.Analytics;

public class ClassFillRateResponse
{
    public required IReadOnlyList<ClassFillRateItemResponse> Items { get; init; }
}
