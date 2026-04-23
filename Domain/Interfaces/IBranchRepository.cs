using app.Domain.Entities;

namespace app.Domain.Interfaces;

public interface IBranchRepository
{
    Task<Branch> AddAsync(Branch branch, CancellationToken cancellationToken = default);
    Task<Branch?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Branch>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Branch?> UpdateAsync(Branch branch, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(ulong id, ulong? deletedByUserId, CancellationToken cancellationToken = default);
}
