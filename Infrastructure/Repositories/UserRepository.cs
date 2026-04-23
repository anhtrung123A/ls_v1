using app.Domain.Entities;
using app.Domain.Interfaces;
using app.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FileEntity = app.Domain.Entities.File;

namespace app.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetByIdsAsync(IEnumerable<ulong> ids, CancellationToken cancellationToken = default)
    {
        var userIds = ids.Distinct().ToArray();
        if (userIds.Length == 0)
        {
            return [];
        }

        return await _dbContext.Users
            .AsNoTracking()
            .Where(x => userIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyDictionary<ulong, string>> GetFileObjectKeysByIdsAsync(IEnumerable<ulong> fileIds, CancellationToken cancellationToken = default)
    {
        var ids = fileIds.Distinct().ToArray();
        if (ids.Length == 0)
        {
            return new Dictionary<ulong, string>();
        }

        var items = await _dbContext.Files
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Select(x => new { x.Id, x.ObjectKey })
            .ToListAsync(cancellationToken);

        return items.ToDictionary(x => x.Id, x => x.ObjectKey);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task AssignRoleAsync(ulong userId, ulong roleId, ulong? createdByUserId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var exists = await _dbContext.UserRoles
                .AsNoTracking()
                .AnyAsync(x => x.UserId == userId && x.RoleId == roleId, cancellationToken);

            if (exists)
            {
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            var now = DateTime.UtcNow;
            _dbContext.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                AssignedAt = now,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedByUserId = createdByUserId,
                UpdatedByUserId = createdByUserId
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<(User? User, FileEntity? Avatar)> GetWithAvatarByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user is null || !user.AvatarFileId.HasValue)
        {
            return (user, null);
        }

        var avatar = await _dbContext.Files
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == user.AvatarFileId.Value, cancellationToken);

        return (user, avatar);
    }

    public async Task<ulong?> GetRoleIdByUserIdAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.AssignedAt)
            .Select(x => (ulong?)x.RoleId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ulong?> GetRoleIdByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Email == email)
            .Select(x => x.Id)
            .SelectMany(userId => _dbContext.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .OrderByDescending(ur => ur.AssignedAt)
                .Select(ur => (ulong?)ur.RoleId)
                .Take(1))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FileEntity?> UpsertAvatarAsync(ulong userId, FileEntity newAvatar, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var currentAvatarFileId = await _dbContext.Users
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => x.AvatarFileId)
                .FirstOrDefaultAsync(cancellationToken);

            var userExists = await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(x => x.Id == userId, cancellationToken);
            if (!userExists)
            {
                throw new KeyNotFoundException("User not found.");
            }

            FileEntity? previousAvatar = null;
            if (currentAvatarFileId.HasValue)
            {
                previousAvatar = await _dbContext.Files
                    .FirstOrDefaultAsync(x => x.Id == currentAvatarFileId.Value, cancellationToken);
            }

            _dbContext.Files.Add(newAvatar);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _dbContext.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.AvatarFileId, newAvatar.Id), cancellationToken);

            if (previousAvatar is not null)
            {
                previousAvatar.DeletedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return previousAvatar;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<FileEntity?> RemoveAvatarAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        var avatarFileId = await _dbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.AvatarFileId)
            .FirstOrDefaultAsync(cancellationToken);

        var userExists = await _dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == userId, cancellationToken);
        if (!userExists)
        {
            throw new KeyNotFoundException("User not found.");
        }

        if (!avatarFileId.HasValue)
        {
            return null;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existingAvatar = await _dbContext.Files
                .FirstOrDefaultAsync(x => x.Id == avatarFileId.Value, cancellationToken);

            await _dbContext.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.AvatarFileId, (ulong?)null), cancellationToken);

            if (existingAvatar is not null)
            {
                existingAvatar.DeletedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return existingAvatar;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
