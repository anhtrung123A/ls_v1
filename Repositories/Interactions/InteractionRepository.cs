using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Interactions;

namespace app.Repositories.Interactions;

public class InteractionRepository : IInteractionRepository
{
    private readonly AppDbContext _dbContext;

    public InteractionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<InteractionResponse>> GetAllAsync(InteractionListQuery query, CancellationToken cancellationToken = default)
    {
        var q = BuildQuery(query);
        return await ToPagedAsync(q, query, cancellationToken);
    }

    public async Task<PagedResponse<InteractionResponse>> GetByLeadIdAsync(long leadId, InteractionListQuery query, CancellationToken cancellationToken = default)
    {
        query.LeadId = leadId;
        var q = BuildQuery(query);
        return await ToPagedAsync(q, query, cancellationToken);
    }

    public async Task<InteractionResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Interactions.AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<InteractionResponse> CreateAsync(InteractionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Interaction
        {
            LeadId = request.LeadId,
            StaffId = request.StaffId,
            Channel = request.Channel,
            Direction = request.Direction,
            Content = request.Content,
            Outcome = request.Outcome,
            Attachments = request.Attachments,
            OccurredAt = request.OccurredAt ?? DateTime.UtcNow
        };

        _dbContext.Interactions.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created interaction.");
    }

    public async Task<InteractionResponse?> UpdateAsync(long id, InteractionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Interactions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.LeadId = request.LeadId;
        entity.StaffId = request.StaffId;
        entity.Channel = request.Channel;
        entity.Direction = request.Direction;
        entity.Content = request.Content;
        entity.Outcome = request.Outcome;
        entity.Attachments = request.Attachments;
        entity.OccurredAt = request.OccurredAt ?? entity.OccurredAt;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Interactions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException("Interaction not found.");
        }

        _dbContext.Interactions.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Interaction> BuildQuery(InteractionListQuery query)
    {
        var q = _dbContext.Interactions.AsNoTracking().AsQueryable();

        if (query.LeadId.HasValue)
        {
            q = q.Where(x => x.LeadId == query.LeadId.Value);
        }

        if (query.StaffId.HasValue)
        {
            q = q.Where(x => x.StaffId == query.StaffId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Channel))
        {
            var channel = query.Channel.Trim().ToLowerInvariant();
            q = q.Where(x => x.Channel != null && x.Channel.ToLower() == channel);
        }

        if (!string.IsNullOrWhiteSpace(query.Direction))
        {
            var direction = query.Direction.Trim().ToLowerInvariant();
            q = q.Where(x => x.Direction != null && x.Direction.ToLower() == direction);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x =>
                (x.Content != null && x.Content.ToLower().Contains(keyword)) ||
                (x.Outcome != null && x.Outcome.ToLower().Contains(keyword)) ||
                (x.Channel != null && x.Channel.ToLower().Contains(keyword)));
        }

        return ApplyOrdering(q, query.OrderBy, query.OrderDir);
    }

    private async Task<PagedResponse<InteractionResponse>> ToPagedAsync(IQueryable<Interaction> q, InteractionListQuery query, CancellationToken cancellationToken)
    {
        var totalItems = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<InteractionResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    private static IQueryable<Interaction> ApplyOrdering(IQueryable<Interaction> query, string? orderBy, string? orderDir)
    {
        var isAsc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();

        return key switch
        {
            "channel" => isAsc ? query.OrderBy(x => x.Channel) : query.OrderByDescending(x => x.Channel),
            "direction" => isAsc ? query.OrderBy(x => x.Direction) : query.OrderByDescending(x => x.Direction),
            _ => isAsc ? query.OrderBy(x => x.OccurredAt) : query.OrderByDescending(x => x.OccurredAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<Interaction, InteractionResponse>> Map() => x => new InteractionResponse
    {
        Id = x.Id,
        LeadId = x.LeadId,
        StaffId = x.StaffId,
        Channel = x.Channel,
        Direction = x.Direction,
        Content = x.Content,
        Outcome = x.Outcome,
        Attachments = x.Attachments,
        OccurredAt = x.OccurredAt
    };
}
