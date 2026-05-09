using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Courses;

namespace app.Repositories.Courses;

public class CourseRepository : ICourseRepository
{
    private readonly AppDbContext _db;
    public CourseRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<CourseResponse>> GetAllAsync(CourseListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<Course>().AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var k = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x => x.Code.ToLower().Contains(k) || x.Name.ToLower().Contains(k));
        }
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);
        if (query.CategoryId.HasValue) q = q.Where(x => x.CategoryId == query.CategoryId.Value);
        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<CourseResponse> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalItems = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }

    public async Task<CourseResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Set<Course>().AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);

    public async Task<CourseResponse> CreateAsync(CourseRequest request, CancellationToken cancellationToken = default)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        var existed = await _db.Set<Course>().AnyAsync(x => x.Code == code, cancellationToken);
        if (existed) throw new InvalidOperationException("Course code already exists.");
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _db.Set<CourseCategory>().AnyAsync(x => x.Id == request.CategoryId.Value, cancellationToken);
            if (!categoryExists) throw new InvalidOperationException("Course category does not exist.");
        }

        var entity = new Course
        {
            Code = code,
            Name = request.Name.Trim(),
            Description = request.Description,
            CategoryId = request.CategoryId,
            Level = request.Level,
            TotalSessions = request.TotalSessions,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "VND" : request.Currency.Trim().ToUpperInvariant(),
            ThumbnailUrl = request.ThumbnailUrl,
            Status = request.Status ?? 1,
            CreatedBy = request.CreatedBy
        };
        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created course.");
    }

    public async Task<CourseResponse?> UpdateAsync(long id, CourseRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Course>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        var code = request.Code.Trim().ToUpperInvariant();
        var existed = await _db.Set<Course>().AnyAsync(x => x.Id != id && x.Code == code, cancellationToken);
        if (existed) throw new InvalidOperationException("Course code already exists.");

        entity.Code = code;
        entity.Name = request.Name.Trim();
        entity.Description = request.Description;
        entity.CategoryId = request.CategoryId;
        entity.Level = request.Level;
        entity.TotalSessions = request.TotalSessions;
        entity.DurationMinutes = request.DurationMinutes;
        entity.Price = request.Price;
        entity.Currency = string.IsNullOrWhiteSpace(request.Currency) ? "VND" : request.Currency.Trim().ToUpperInvariant();
        entity.ThumbnailUrl = request.ThumbnailUrl;
        entity.Status = request.Status ?? entity.Status;
        entity.CreatedBy = request.CreatedBy;
        if (request.CategoryId.HasValue)
        {
            var categoryExists = await _db.Set<CourseCategory>().AnyAsync(x => x.Id == request.CategoryId.Value, cancellationToken);
            if (!categoryExists) throw new InvalidOperationException("Course category does not exist.");
        }
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Course>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Course not found.");
        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Course> ApplyOrdering(IQueryable<Course> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "code" => asc ? q.OrderBy(x => x.Code) : q.OrderByDescending(x => x.Code),
            "name" => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
            "status" => asc ? q.OrderBy(x => x.Status) : q.OrderByDescending(x => x.Status),
            _ => asc ? q.OrderBy(x => x.CreatedAt) : q.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<Course, CourseResponse>> Map() => x => new CourseResponse
    {
        Id = x.Id, Code = x.Code, Name = x.Name, Description = x.Description, CategoryId = x.CategoryId, Level = x.Level,
        TotalSessions = x.TotalSessions, DurationMinutes = x.DurationMinutes, Price = x.Price, Currency = x.Currency,
        ThumbnailUrl = x.ThumbnailUrl, Status = x.Status, CreatedBy = x.CreatedBy, CreatedAt = x.CreatedAt
    };
}
