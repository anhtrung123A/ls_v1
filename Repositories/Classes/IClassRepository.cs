using app.Common.Pagination;
using app.DTOs.Classes;

namespace app.Repositories.Classes;

public interface IClassRepository
{
    Task<PagedResponse<ClassResponse>> GetAllAsync(ClassListQuery query, CancellationToken cancellationToken = default);
    Task<ClassResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<ClassResponse> CreateAsync(ClassRequest request, CancellationToken cancellationToken = default);
    Task<ClassResponse?> UpdateAsync(long id, ClassRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
