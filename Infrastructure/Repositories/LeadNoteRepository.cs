using app.Domain.Entities;
using app.Domain.Interfaces;
using app.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace app.Infrastructure.Repositories;

public class LeadNoteRepository : ILeadNoteRepository
{
    private readonly AppDbContext _dbContext;

    public LeadNoteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LeadNote> AddAsync(LeadNote leadNote, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            _dbContext.LeadNotes.Add(leadNote);
            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return leadNote;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public Task<LeadNote?> GetByIdAsync(ulong leadId, ulong id, CancellationToken cancellationToken = default)
    {
        return _dbContext.LeadNotes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.LeadId == leadId && x.Id == id, cancellationToken);
    }

    public Task<int> CountByLeadIdAsync(ulong leadId, CancellationToken cancellationToken = default)
    {
        return _dbContext.LeadNotes
            .AsNoTracking()
            .Where(x => x.LeadId == leadId)
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LeadNote>> GetPageByLeadIdAsync(ulong leadId, int offset, int limit, CancellationToken cancellationToken = default)
    {
        return await _dbContext.LeadNotes
            .AsNoTracking()
            .Where(x => x.LeadId == leadId)
            .OrderByDescending(x => x.Id)
            .Skip(offset)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<LeadNote?> UpdateAsync(LeadNote leadNote, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existing = await _dbContext.LeadNotes
                .FirstOrDefaultAsync(x => x.LeadId == leadNote.LeadId && x.Id == leadNote.Id, cancellationToken);
            if (existing is null)
            {
                return null;
            }

            existing.Content = leadNote.Content;
            existing.Metadata = leadNote.Metadata;
            existing.UpdatedAt = leadNote.UpdatedAt;
            existing.UpdatedByUserId = leadNote.UpdatedByUserId;

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

    public async Task<bool> SoftDeleteAsync(ulong leadId, ulong id, ulong? deletedByUserId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var existing = await _dbContext.LeadNotes
                .FirstOrDefaultAsync(x => x.LeadId == leadId && x.Id == id, cancellationToken);
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
