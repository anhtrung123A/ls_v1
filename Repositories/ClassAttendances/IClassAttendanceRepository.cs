using app.Common.Pagination;
using app.DTOs.ClassAttendances;

namespace app.Repositories.ClassAttendances;

public interface IClassAttendanceRepository
{
    Task<PagedResponse<ClassAttendanceResponse>> GetAllAsync(ClassAttendanceListQuery query, CancellationToken cancellationToken = default);
    Task<ClassAttendanceResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClassAttendanceResponse>> BulkCreateAsync(IReadOnlyCollection<ClassAttendanceRequest> requests, long recordedBy, CancellationToken cancellationToken = default);
    Task<ClassAttendanceResponse?> UpdateAsync(long id, ClassAttendanceRequest request, long recordedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
