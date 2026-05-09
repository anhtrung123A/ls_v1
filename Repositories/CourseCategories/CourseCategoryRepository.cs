using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.CourseCategories;

namespace app.Repositories.CourseCategories;

public class CourseCategoryRepository : ICourseCategoryRepository
{
    private readonly AppDbContext _db;
    public CourseCategoryRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<CourseCategoryResponse>> GetAllAsync(CourseCategoryListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<CourseCategory>().AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var k = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x => x.Name.ToLower().Contains(k) || (x.Slug != null && x.Slug.ToLower().Contains(k)));
        }
        if (query.IsActive.HasValue) q = q.Where(x => x.IsActive == query.IsActive.Value);
        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<CourseCategoryResponse> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalItems = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }

    public async Task<CourseCategoryResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Set<CourseCategory>().AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);

    public async Task<CourseCategoryResponse> CreateAsync(CourseCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var slug = await ResolveUniqueSlugAsync(request.Slug, request.Name, null, cancellationToken);
        var entity = new CourseCategory
        {
            Name = request.Name.Trim(),
            Slug = slug,
            Description = request.Description,
            SortOrder = request.SortOrder ?? 0,
            IsActive = request.IsActive ?? true
        };

        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created course category.");
    }

    public async Task<CourseCategoryResponse?> UpdateAsync(long id, CourseCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<CourseCategory>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        entity.Name = request.Name.Trim();
        entity.Slug = await ResolveUniqueSlugAsync(request.Slug, request.Name, id, cancellationToken);
        entity.Description = request.Description;
        entity.SortOrder = request.SortOrder ?? entity.SortOrder;
        entity.IsActive = request.IsActive ?? entity.IsActive;

        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<CourseCategory>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Course category not found.");
        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<string> ResolveUniqueSlugAsync(string? inputSlug, string name, long? currentId, CancellationToken cancellationToken)
    {
        var baseSlug = Slugify(string.IsNullOrWhiteSpace(inputSlug) ? name : inputSlug);
        var slug = baseSlug;
        var i = 1;
        while (await _db.Set<CourseCategory>().AnyAsync(x => x.Slug == slug && (!currentId.HasValue || x.Id != currentId.Value), cancellationToken))
        {
            slug = $"{baseSlug}-{i++}";
        }
        return slug;
    }

    private static string Slugify(string input)
    {
        var slug = input.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }

    private static IQueryable<CourseCategory> ApplyOrdering(IQueryable<CourseCategory> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "name" => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
            "sortorder" => asc ? q.OrderBy(x => x.SortOrder) : q.OrderByDescending(x => x.SortOrder),
            _ => asc ? q.OrderBy(x => x.Id) : q.OrderByDescending(x => x.Id)
        };
    }

    private static System.Linq.Expressions.Expression<Func<CourseCategory, CourseCategoryResponse>> Map() => x => new CourseCategoryResponse
    {
        Id = x.Id,
        Name = x.Name,
        Slug = x.Slug,
        Description = x.Description,
        SortOrder = x.SortOrder,
        IsActive = x.IsActive
    };
}
