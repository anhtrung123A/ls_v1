using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.SalaryConfigs;

namespace app.Repositories.SalaryConfigs;

public class SalaryConfigRepository : ISalaryConfigRepository
{
    private readonly AppDbContext _db;
    public SalaryConfigRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<SalaryConfigResponse>> GetAllAsync(SalaryConfigListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<SalaryConfig>().AsNoTracking().AsQueryable();
        if (query.UserId.HasValue) q = q.Where(x => x.UserId == query.UserId.Value);
        if (query.SalaryType.HasValue) q = q.Where(x => x.SalaryType == query.SalaryType.Value);
        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<SalaryConfigResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize)
        };
    }

    public async Task<SalaryConfigResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Set<SalaryConfig>().AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);

    public async Task<SalaryConfigResponse> CreateAsync(SalaryConfigRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateUserAsync(request.UserId, cancellationToken);
        ValidateDateRange(request.EffectiveFrom, request.EffectiveTo);

        var entity = new SalaryConfig
        {
            UserId = request.UserId,
            SalaryType = request.SalaryType,
            BaseSalary = request.BaseSalary ?? 0m,
            TeachingRate = request.TeachingRate ?? 0m,
            ConvertedLeadRate = request.ConvertedLeadRate ?? 0m,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            IsActive = true
        };

        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created salary config.");
    }

    public async Task<SalaryConfigResponse?> UpdateAsync(long id, SalaryConfigRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<SalaryConfig>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        await ValidateUserAsync(request.UserId, cancellationToken);
        ValidateDateRange(request.EffectiveFrom, request.EffectiveTo);

        entity.UserId = request.UserId;
        entity.SalaryType = request.SalaryType;
        entity.BaseSalary = request.BaseSalary ?? entity.BaseSalary;
        entity.TeachingRate = request.TeachingRate ?? entity.TeachingRate;
        entity.ConvertedLeadRate = request.ConvertedLeadRate ?? entity.ConvertedLeadRate;
        entity.EffectiveFrom = request.EffectiveFrom;
        entity.EffectiveTo = request.EffectiveTo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<SalaryConfig>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Salary config not found.");
        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateUserAsync(long userId, CancellationToken cancellationToken)
    {
        var existed = await _db.Set<User>().AnyAsync(x => x.Id == userId, cancellationToken);
        if (!existed) throw new InvalidOperationException("User does not exist.");
    }

    private static void ValidateDateRange(DateOnly from, DateOnly? to)
    {
        if (to.HasValue && to.Value < from)
        {
            throw new InvalidOperationException("effective_to must be greater than or equal to effective_from.");
        }
    }

    private static IQueryable<SalaryConfig> ApplyOrdering(IQueryable<SalaryConfig> q, string? orderBy, string? orderDir)
    {
        var asc = !string.Equals(orderDir, "desc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "userid" or "user_id" => asc ? q.OrderBy(x => x.UserId) : q.OrderByDescending(x => x.UserId),
            "effectivedate" or "effective_from" => asc ? q.OrderBy(x => x.EffectiveFrom) : q.OrderByDescending(x => x.EffectiveFrom),
            _ => asc ? q.OrderBy(x => x.Id) : q.OrderByDescending(x => x.Id)
        };
    }

    private static System.Linq.Expressions.Expression<Func<SalaryConfig, SalaryConfigResponse>> Map() => x => new SalaryConfigResponse
    {
        Id = x.Id,
        UserId = x.UserId,
        SalaryType = x.SalaryType,
        BaseSalary = x.BaseSalary,
        TeachingRate = x.TeachingRate,
        ConvertedLeadRate = x.ConvertedLeadRate,
        EffectiveFrom = x.EffectiveFrom,
        EffectiveTo = x.EffectiveTo,
        IsActive = x.IsActive,
        CreatedAt = x.CreatedAt,
        UpdatedAt = x.UpdatedAt
    };
}
