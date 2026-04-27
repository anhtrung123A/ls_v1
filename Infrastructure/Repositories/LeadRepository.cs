using app.Domain.Entities;
using app.Domain.Interfaces;
using app.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace app.Infrastructure.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly AppDbContext _dbContext;

    public LeadRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Lead> AddAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _dbContext.Leads.Add(lead);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return lead;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<Lead?> GetByIdAsync(ulong id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Leads
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Leads.AsNoTracking().CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Lead>> GetPageAsync(int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Leads
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAssignedToAsync(ulong userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Leads
            .AsNoTracking()
            .Where(x => x.AssignedTo == userId)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Lead>> GetAssignedPageAsync(ulong userId, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Leads
            .AsNoTracking()
            .Where(x => x.AssignedTo == userId)
            .OrderByDescending(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Lead?> UpdateAsync(Lead lead, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existing = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == lead.Id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            existing.FirstName = lead.FirstName;
            existing.FullName = lead.FullName;
            existing.Source = lead.Source;
            existing.Status = lead.Status;
            existing.AssignedTo = lead.AssignedTo;
            existing.Note = lead.Note;
            existing.Metadata = lead.Metadata;
            existing.UpdatedAt = lead.UpdatedAt;
            existing.UpdatedByUserId = lead.UpdatedByUserId;

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
            var existing = await _dbContext.Leads.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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
