using app.Common.Pagination;
using app.DTOs.Enrollments;

namespace app.Repositories.Enrollments;

public interface IEnrollmentRepository
{
    Task<PagedResponse<EnrollmentResponse>> GetAllAsync(EnrollmentListQuery query, CancellationToken cancellationToken = default);
    Task<EnrollmentResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<EnrollmentResponse> CreateAsync(EnrollmentRequest request, long enrolledByStaffId, CancellationToken cancellationToken = default);
    Task<EnrollmentResponse?> UpdateAsync(long id, EnrollmentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
