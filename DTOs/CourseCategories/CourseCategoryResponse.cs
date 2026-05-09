namespace app.DTOs.CourseCategories;

public class CourseCategoryResponse
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Slug { get; init; }
    public string? Description { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; }
}
