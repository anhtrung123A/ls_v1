using app.Common.Pagination;
using app.DTOs.Students;

namespace app.Repositories.Students;

public interface IStudentRepository
{
    Task<PagedResponse<StudentResponse>> GetAllAsync(StudentListQuery query, CancellationToken cancellationToken = default);
    Task<StudentResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse?> UpdateAsync(long id, UpdateStudentRequest request, CancellationToken cancellationToken = default);
    Task<StudentResponse?> CreateUserAsync(long id, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
