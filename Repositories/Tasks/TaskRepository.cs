using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Tasks;

namespace app.Repositories.Tasks;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _dbContext;

    public TaskRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<TaskResponse>> GetAllAsync(TaskListQuery query, CancellationToken cancellationToken = default)
    {
        var q = BuildQuery(query);
        return await ToPagedAsync(q, query, cancellationToken);
    }

    public async Task<PagedResponse<TaskResponse>> GetByLeadIdAsync(long leadId, TaskListQuery query, CancellationToken cancellationToken = default)
    {
        query.RelatedLeadId = leadId;
        var q = BuildQuery(query);
        return await ToPagedAsync(q, query, cancellationToken);
    }

    public async Task<TaskResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Tasks.AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new TaskItem
        {
            AssignedTo = request.AssignedTo,
            CreatedBy = request.CreatedBy,
            RelatedLeadId = request.RelatedLeadId,
            Title = request.Title.Trim(),
            Description = request.Description,
            Type = request.Type,
            Priority = request.Priority ?? 2,
            Status = request.Status ?? 1,
            DueAt = request.DueAt
        };

        _dbContext.Tasks.Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created task.");
    }

    public async Task<TaskResponse?> UpdateAsync(long id, TaskRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.AssignedTo = request.AssignedTo;
        entity.CreatedBy = request.CreatedBy;
        entity.RelatedLeadId = request.RelatedLeadId;
        entity.Title = request.Title.Trim();
        entity.Description = request.Description;
        entity.Type = request.Type;
        entity.Priority = request.Priority ?? entity.Priority;
        entity.Status = request.Status ?? entity.Status;
        entity.DueAt = request.DueAt;
        entity.DoneAt = request.DoneAt;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            throw new KeyNotFoundException("Task not found.");
        }

        _dbContext.Tasks.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<TaskItem> BuildQuery(TaskListQuery query)
    {
        var q = _dbContext.Tasks.AsNoTracking().AsQueryable();

        if (query.RelatedLeadId.HasValue)
        {
            q = q.Where(x => x.RelatedLeadId == query.RelatedLeadId.Value);
        }

        if (query.AssignedTo.HasValue)
        {
            q = q.Where(x => x.AssignedTo == query.AssignedTo.Value);
        }

        if (query.CreatedBy.HasValue)
        {
            q = q.Where(x => x.CreatedBy == query.CreatedBy.Value);
        }

        if (query.Priority.HasValue)
        {
            q = q.Where(x => x.Priority == query.Priority.Value);
        }

        if (query.Status.HasValue)
        {
            q = q.Where(x => x.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x =>
                x.Title.ToLower().Contains(keyword) ||
                (x.Description != null && x.Description.ToLower().Contains(keyword)) ||
                (x.Type != null && x.Type.ToLower().Contains(keyword)));
        }

        return ApplyOrdering(q, query.OrderBy, query.OrderDir);
    }

    private async Task<PagedResponse<TaskResponse>> ToPagedAsync(IQueryable<TaskItem> q, TaskListQuery query, CancellationToken cancellationToken)
    {
        var totalItems = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<TaskResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    private static IQueryable<TaskItem> ApplyOrdering(IQueryable<TaskItem> query, string? orderBy, string? orderDir)
    {
        var isAsc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();

        return key switch
        {
            "title" => isAsc ? query.OrderBy(x => x.Title) : query.OrderByDescending(x => x.Title),
            "priority" => isAsc ? query.OrderBy(x => x.Priority) : query.OrderByDescending(x => x.Priority),
            "status" => isAsc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            "dueat" => isAsc ? query.OrderBy(x => x.DueAt) : query.OrderByDescending(x => x.DueAt),
            _ => isAsc ? query.OrderBy(x => x.CreatedAt) : query.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<TaskItem, TaskResponse>> Map() => x => new TaskResponse
    {
        Id = x.Id,
        AssignedTo = x.AssignedTo,
        CreatedBy = x.CreatedBy,
        RelatedLeadId = x.RelatedLeadId,
        Title = x.Title,
        Description = x.Description,
        Type = x.Type,
        Priority = x.Priority,
        Status = x.Status,
        DueAt = x.DueAt,
        DoneAt = x.DoneAt,
        CreatedAt = x.CreatedAt
    };
}
