using app.Common.Pagination;
using app.DTOs.Courses;

namespace app.Repositories.Courses;

public interface ICourseRepository
{
    Task<PagedResponse<CourseResponse>> GetAllAsync(CourseListQuery query, CancellationToken cancellationToken = default);
    Task<CourseResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<CourseResponse> CreateAsync(CourseRequest request, CancellationToken cancellationToken = default);
    Task<CourseResponse?> UpdateAsync(long id, CourseRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
