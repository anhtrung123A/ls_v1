using app.Domain.Entities;

namespace app.Domain.Interfaces;

public interface ILeadRepository
{
    Task<Lead> AddAsync(Lead lead, CancellationToken cancellationToken = default);
    Task<Lead?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lead>> GetPageAsync(int offset, int limit, CancellationToken cancellationToken = default);
    Task<int> CountAssignedToAsync(ulong userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Lead>> GetAssignedPageAsync(ulong userId, int offset, int limit, CancellationToken cancellationToken = default);
    Task<Lead?> UpdateAsync(Lead lead, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(ulong id, ulong? deletedByUserId, CancellationToken cancellationToken = default);
}
