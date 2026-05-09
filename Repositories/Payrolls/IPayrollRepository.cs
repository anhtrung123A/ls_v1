using app.Common.Pagination;
using app.DTOs.Payrolls;

namespace app.Repositories.Payrolls;

public interface IPayrollRepository
{
    Task<PagedResponse<PayrollResponse>> GetAllAsync(PayrollListQuery query, CancellationToken cancellationToken = default);
    Task<PayrollDetailResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PayrollResponse?> UpdateStatusAsync(long id, UpdatePayrollStatusRequest request, CancellationToken cancellationToken = default);
}
