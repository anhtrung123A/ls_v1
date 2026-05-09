using app.Common.Pagination;
using app.DTOs.SalaryConfigs;

namespace app.Repositories.SalaryConfigs;

public interface ISalaryConfigRepository
{
    Task<PagedResponse<SalaryConfigResponse>> GetAllAsync(SalaryConfigListQuery query, CancellationToken cancellationToken = default);
    Task<SalaryConfigResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<SalaryConfigResponse> CreateAsync(SalaryConfigRequest request, CancellationToken cancellationToken = default);
    Task<SalaryConfigResponse?> UpdateAsync(long id, SalaryConfigRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
