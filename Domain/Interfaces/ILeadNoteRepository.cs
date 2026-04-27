using app.Domain.Entities;

namespace app.Domain.Interfaces;

public interface ILeadNoteRepository
{
    Task<LeadNote> AddAsync(LeadNote leadNote, CancellationToken cancellationToken = default);
    Task<LeadNote?> GetByIdAsync(ulong leadId, ulong id, CancellationToken cancellationToken = default);
    Task<int> CountByLeadIdAsync(ulong leadId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeadNote>> GetPageByLeadIdAsync(ulong leadId, int offset, int limit, CancellationToken cancellationToken = default);
    Task<LeadNote?> UpdateAsync(LeadNote leadNote, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(ulong leadId, ulong id, ulong? deletedByUserId, CancellationToken cancellationToken = default);
}
