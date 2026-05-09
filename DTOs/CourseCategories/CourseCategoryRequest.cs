namespace app.DTOs.CourseCategories;

public class CourseCategoryRequest
{
    public required string Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; }
}
