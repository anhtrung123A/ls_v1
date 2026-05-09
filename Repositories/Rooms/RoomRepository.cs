using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Rooms;

namespace app.Repositories.Rooms;

public class RoomRepository : IRoomRepository
{
    private readonly AppDbContext _db;
    public RoomRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<RoomResponse>> GetAllAsync(RoomListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<Room>().AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var k = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x => x.Name.ToLower().Contains(k) || (x.Location != null && x.Location.ToLower().Contains(k)));
        }
        if (query.IsActive.HasValue) q = q.Where(x => x.IsActive == query.IsActive.Value);
        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<RoomResponse> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalItems = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }

    public async Task<RoomResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Set<Room>().AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);

    public async Task<RoomResponse> CreateAsync(RoomRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Room { Name = request.Name.Trim(), Location = request.Location, Capacity = request.Capacity, Facilities = request.Facilities, IsActive = request.IsActive ?? true };
        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created room.");
    }

    public async Task<RoomResponse?> UpdateAsync(long id, RoomRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Room>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        entity.Name = request.Name.Trim();
        entity.Location = request.Location;
        entity.Capacity = request.Capacity;
        entity.Facilities = request.Facilities;
        entity.IsActive = request.IsActive ?? entity.IsActive;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Room>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Room not found.");
        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Room> ApplyOrdering(IQueryable<Room> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "name" => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
            "capacity" => asc ? q.OrderBy(x => x.Capacity) : q.OrderByDescending(x => x.Capacity),
            _ => asc ? q.OrderBy(x => x.CreatedAt) : q.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<Room, RoomResponse>> Map() => x => new RoomResponse
    {
        Id = x.Id, Name = x.Name, Location = x.Location, Capacity = x.Capacity, Facilities = x.Facilities, IsActive = x.IsActive, CreatedAt = x.CreatedAt
    };
}
