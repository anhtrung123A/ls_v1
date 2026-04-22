using app.Domain.Entities;
using FileEntity = app.Domain.Entities.File;

namespace app.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<(User? User, FileEntity? Avatar)> GetWithAvatarByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<FileEntity?> UpsertAvatarAsync(ulong userId, FileEntity newAvatar, CancellationToken cancellationToken = default);
    Task<FileEntity?> RemoveAvatarAsync(ulong userId, CancellationToken cancellationToken = default);
}
