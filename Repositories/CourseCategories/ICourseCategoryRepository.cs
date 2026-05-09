using app.Common.Pagination;
using app.DTOs.CourseCategories;

namespace app.Repositories.CourseCategories;

public interface ICourseCategoryRepository
{
    Task<PagedResponse<CourseCategoryResponse>> GetAllAsync(CourseCategoryListQuery query, CancellationToken cancellationToken = default);
    Task<CourseCategoryResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<CourseCategoryResponse> CreateAsync(CourseCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CourseCategoryResponse?> UpdateAsync(long id, CourseCategoryRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
