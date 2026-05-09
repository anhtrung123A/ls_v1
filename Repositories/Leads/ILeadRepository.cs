using app.DTOs.Leads;
using app.Common.Pagination;

namespace app.Repositories.Leads;

public interface ILeadRepository
{
    Task<PagedResponse<LeadResponse>> GetAllAsync(LeadListQuery query, CancellationToken cancellationToken = default);
    Task<LeadResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<LeadResponse> CreateAsync(CreateLeadRequest request, CancellationToken cancellationToken = default);
    Task<LeadResponse?> UpdateAsync(long id, UpdateLeadRequest request, CancellationToken cancellationToken = default);
    Task<LeadResponse?> AssignAsync(long id, long? assignedTo, CancellationToken cancellationToken = default);
    Task<ConvertLeadResponse?> ConvertToStudentAsync(long id, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
