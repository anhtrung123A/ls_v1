using app.Domain.Entities;
using FileEntity = app.Domain.Entities.File;

namespace app.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<ulong> ids, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<ulong, string>> GetFileObjectKeysByIdsAsync(IEnumerable<ulong> fileIds, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task AssignRoleAsync(ulong userId, ulong roleId, ulong? createdByUserId, CancellationToken cancellationToken = default);
    Task<(User? User, FileEntity? Avatar)> GetWithAvatarByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ulong?> GetRoleIdByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ulong?> GetRoleIdByUserIdAsync(ulong userId, CancellationToken cancellationToken = default);
    Task<FileEntity?> UpsertAvatarAsync(ulong userId, FileEntity newAvatar, CancellationToken cancellationToken = default);
    Task<FileEntity?> RemoveAvatarAsync(ulong userId, CancellationToken cancellationToken = default);
}
