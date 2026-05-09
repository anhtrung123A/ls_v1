using app.Common.Pagination;
using app.DTOs.ClassSchedules;

namespace app.Repositories.ClassSchedules;

public interface IClassScheduleRepository
{
    Task<PagedResponse<ClassScheduleResponse>> GetAllAsync(ClassScheduleListQuery query, CancellationToken cancellationToken = default);
    Task<PagedResponse<ClassScheduleResponse>> GetByClassIdAsync(long classId, ClassScheduleListQuery query, CancellationToken cancellationToken = default);
    Task<ClassScheduleResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ClassScheduleResponse> CreateAsync(ClassScheduleRequest request, CancellationToken cancellationToken = default);
    Task<ClassScheduleResponse?> UpdateAsync(long id, ClassScheduleRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
