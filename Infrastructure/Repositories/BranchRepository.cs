using app.Domain.Entities;
using app.Domain.Interfaces;
using app.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using FileEntity = app.Domain.Entities.File;

namespace app.Infrastructure.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly AppDbContext _dbContext;

    public BranchRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Branch> AddAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _dbContext.Branches.Add(branch);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return branch;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<Branch?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Branch>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Branches
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Branch?> UpdateAsync(Branch branch, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existing = await _dbContext.Branches
                .FirstOrDefaultAsync(x => x.Id == branch.Id, cancellationToken);

            if (existing is null)
            {
                return null;
            }

            existing.Name = branch.Name;
            existing.Description = branch.Description;
            existing.AddressLine1 = branch.AddressLine1;
            existing.AddressLine2 = branch.AddressLine2;
            existing.Ward = branch.Ward;
            existing.District = branch.District;
            existing.City = branch.City;
            existing.PostalCode = branch.PostalCode;
            existing.Country = branch.Country;
            existing.ImageFileId = branch.ImageFileId;
            existing.UpdatedAt = branch.UpdatedAt;
            existing.UpdatedByUserId = branch.UpdatedByUserId;

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
            var existing = await _dbContext.Branches
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

    public async Task<string?> GetImageObjectKeyByFileIdAsync(ulong fileId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Files
            .AsNoTracking()
            .Where(x => x.Id == fileId)
            .Select(x => x.ObjectKey)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<FileEntity?> UpsertImageAsync(
        ulong branchId,
        FileEntity newImage,
        ulong? updatedByUserId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existingBranch = await _dbContext.Branches
                .FirstOrDefaultAsync(x => x.Id == branchId, cancellationToken);

            if (existingBranch is null)
            {
                throw new KeyNotFoundException("Branch not found.");
            }

            FileEntity? previousImage = null;
            if (existingBranch.ImageFileId.HasValue)
            {
                previousImage = await _dbContext.Files
                    .FirstOrDefaultAsync(x => x.Id == existingBranch.ImageFileId.Value, cancellationToken);
            }

            _dbContext.Files.Add(newImage);
            await _dbContext.SaveChangesAsync(cancellationToken);

            existingBranch.ImageFileId = newImage.Id;
            existingBranch.UpdatedAt = DateTime.UtcNow;
            existingBranch.UpdatedByUserId = updatedByUserId;

            if (previousImage is not null)
            {
                previousImage.DeletedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return previousImage;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
