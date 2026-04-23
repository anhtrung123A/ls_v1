using app.Domain.Entities;
using app.Domain.Interfaces;
using app.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace app.Infrastructure.Repositories;

public class BranchUserRepository : IBranchUserRepository
{
    private readonly AppDbContext _dbContext;

    public BranchUserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsAsync(ulong userId, ulong branchId, CancellationToken cancellationToken = default)
    {
        return _dbContext.BranchUsers
            .AsNoTracking()
            .AnyAsync(x => x.UserId == userId && x.BranchId == branchId, cancellationToken);
    }

    public async Task<BranchUser> AddAsync(BranchUser branchUser, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _dbContext.BranchUsers.Add(branchUser);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return branchUser;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<BranchUser?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        return _dbContext.BranchUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<BranchUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.BranchUsers
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<BranchUser>> GetByBranchIdAsync(ulong branchId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BranchUsers
            .AsNoTracking()
            .Where(x => x.BranchId == branchId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountUsersAsync(ulong? branchId, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.BranchUsers.AsNoTracking();
        if (branchId.HasValue)
        {
            query = query.Where(x => x.BranchId == branchId.Value);
        }

        return query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<(ulong Id, string? Firstname, string Lastname, string Email, string? Phonenumber, DateOnly? DateOfBirth, ulong BranchId, int Status, string? AvatarObjectKey)>> GetUsersPageAsync(
        ulong? branchId,
        int offset,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var query =
            from bu in _dbContext.BranchUsers.AsNoTracking()
            join u in _dbContext.Users.AsNoTracking() on bu.UserId equals u.Id
            join f in _dbContext.Files.AsNoTracking() on u.AvatarFileId equals (ulong?)f.Id into fileGroup
            from f in fileGroup.DefaultIfEmpty()
            select new
            {
                Id = u.Id,
                Firstname = u.Firstname,
                Lastname = u.Lastname,
                Email = u.Email,
                Phonenumber = u.Phonenumber,
                DateOfBirth = u.DateOfBirth,
                BranchId = bu.BranchId,
                Status = bu.Status,
                AvatarObjectKey = f != null ? f.ObjectKey : null
            };

        if (branchId.HasValue)
        {
            query = query.Where(x => x.BranchId == branchId.Value);
        }

        var rows = await query
            .OrderBy(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return rows
            .Select(x => (
                x.Id,
                (string?)x.Firstname,
                x.Lastname,
                x.Email,
                (string?)x.Phonenumber,
                x.DateOfBirth,
                x.BranchId,
                x.Status,
                (string?)x.AvatarObjectKey))
            .ToList();
    }

    public async Task<(ulong Id, string? Firstname, string Lastname, string Email, string? Phonenumber, DateOnly? DateOfBirth, ulong BranchId, int Status, string? AvatarObjectKey)?> GetUserByBranchUserIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var row = await (
            from bu in _dbContext.BranchUsers.AsNoTracking()
            join u in _dbContext.Users.AsNoTracking() on bu.UserId equals u.Id
            join f in _dbContext.Files.AsNoTracking() on u.AvatarFileId equals (ulong?)f.Id into fileGroup
            from f in fileGroup.DefaultIfEmpty()
            where bu.Id == id
            select new
            {
                Id = u.Id,
                Firstname = u.Firstname,
                Lastname = u.Lastname,
                Email = u.Email,
                Phonenumber = u.Phonenumber,
                DateOfBirth = u.DateOfBirth,
                BranchId = bu.BranchId,
                Status = bu.Status,
                AvatarObjectKey = f != null ? f.ObjectKey : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        return row is null
            ? null
            : (
                row.Id,
                (string?)row.Firstname,
                row.Lastname,
                row.Email,
                (string?)row.Phonenumber,
                row.DateOfBirth,
                row.BranchId,
                row.Status,
                (string?)row.AvatarObjectKey);
    }

    public async Task<BranchUser?> UpdateAsync(BranchUser branchUser, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existing = await _dbContext.BranchUsers
                .FirstOrDefaultAsync(x => x.Id == branchUser.Id, cancellationToken);

            if (existing is null)
            {
                return null;
            }

            existing.Status = branchUser.Status;
            existing.UpdatedAt = branchUser.UpdatedAt;
            existing.UpdatedByUserId = branchUser.UpdatedByUserId;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return existing;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<bool> SoftDeleteAsync(ulong id, ulong? deletedByUserId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existing = await _dbContext.BranchUsers
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (existing is null)
            {
                return false;
            }

            var now = DateTime.UtcNow;
            existing.DeletedAt = now;
            existing.UpdatedAt = now;
            existing.UpdatedByUserId = deletedByUserId;

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
