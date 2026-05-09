using app.Common.Pagination;
using app.DTOs.Interactions;

namespace app.Repositories.Interactions;

public interface IInteractionRepository
{
    Task<PagedResponse<InteractionResponse>> GetAllAsync(InteractionListQuery query, CancellationToken cancellationToken = default);
    Task<PagedResponse<InteractionResponse>> GetByLeadIdAsync(long leadId, InteractionListQuery query, CancellationToken cancellationToken = default);
    Task<InteractionResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<InteractionResponse> CreateAsync(InteractionRequest request, CancellationToken cancellationToken = default);
    Task<InteractionResponse?> UpdateAsync(long id, InteractionRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
