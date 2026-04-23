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

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
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
            .FirstOrDefaultAsync(x => x.Id == user.AvatarFileId.Value && x.DeletedAt == null, cancellationToken);

        return (user, avatar);
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
                    .FirstOrDefaultAsync(x => x.Id == currentAvatarFileId.Value && x.DeletedAt == null, cancellationToken);
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
                .FirstOrDefaultAsync(x => x.Id == avatarFileId.Value && x.DeletedAt == null, cancellationToken);

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
