using app.Common.Pagination;

namespace app.DTOs.CourseCategories;

public class CourseCategoryListQuery : PaginationQuery
{
    public string? Keyword { get; set; }
    public bool? IsActive { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
