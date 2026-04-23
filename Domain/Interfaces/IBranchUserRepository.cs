using app.Domain.Entities;

namespace app.Domain.Interfaces;

public interface IBranchUserRepository
{
    Task<bool> ExistsAsync(ulong userId, ulong branchId, CancellationToken cancellationToken = default);
    Task<BranchUser> AddAsync(BranchUser branchUser, CancellationToken cancellationToken = default);
    Task<BranchUser?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BranchUser>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BranchUser>> GetByBranchIdAsync(ulong branchId, CancellationToken cancellationToken = default);
    Task<int> CountUsersAsync(ulong? branchId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(ulong Id, string? Firstname, string Lastname, string Email, string? Phonenumber, DateOnly? DateOfBirth, ulong BranchId, int Status, string? AvatarObjectKey)>> GetUsersPageAsync(ulong? branchId, int offset, int limit, CancellationToken cancellationToken = default);
    Task<(ulong Id, string? Firstname, string Lastname, string Email, string? Phonenumber, DateOnly? DateOfBirth, ulong BranchId, int Status, string? AvatarObjectKey)?> GetUserByBranchUserIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<BranchUser?> UpdateAsync(BranchUser branchUser, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(ulong id, ulong? deletedByUserId, CancellationToken cancellationToken = default);
}
