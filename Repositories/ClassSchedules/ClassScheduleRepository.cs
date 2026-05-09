using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.ClassSchedules;

namespace app.Repositories.ClassSchedules;

public class ClassScheduleRepository : IClassScheduleRepository
{
    private readonly AppDbContext _db;
    public ClassScheduleRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<ClassScheduleResponse>> GetAllAsync(ClassScheduleListQuery query, CancellationToken cancellationToken = default)
    {
        var q = BuildQuery(query);
        return await ToPagedAsync(q, query, cancellationToken);
    }

    public async Task<PagedResponse<ClassScheduleResponse>> GetByClassIdAsync(long classId, ClassScheduleListQuery query, CancellationToken cancellationToken = default)
    {
        query.ClassId = classId;
        var q = BuildQuery(query);
        return await ToPagedAsync(q, query, cancellationToken);
    }

    public async Task<ClassScheduleResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Set<ClassSchedule>().AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);

    public async Task<ClassScheduleResponse> CreateAsync(ClassScheduleRequest request, CancellationToken cancellationToken = default)
    {
        await Validate(request, cancellationToken);
        var entity = new ClassSchedule
        {
            ClassId = request.ClassId,
            RoomId = request.RoomId,
            Weekday = request.Weekday,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };
        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created class schedule.");
    }

    public async Task<ClassScheduleResponse?> UpdateAsync(long id, ClassScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ClassSchedule>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        await Validate(request, cancellationToken);

        entity.ClassId = request.ClassId;
        entity.RoomId = request.RoomId;
        entity.Weekday = request.Weekday;
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ClassSchedule>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Class schedule not found.");
        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<ClassSchedule> BuildQuery(ClassScheduleListQuery query)
    {
        var q = _db.Set<ClassSchedule>().AsNoTracking().AsQueryable();
        if (query.ClassId.HasValue) q = q.Where(x => x.ClassId == query.ClassId.Value);
        if (query.Weekday.HasValue) q = q.Where(x => x.Weekday == query.Weekday.Value);
        if (query.RoomId.HasValue) q = q.Where(x => x.RoomId == query.RoomId.Value);
        if (query.StartTimeFrom.HasValue) q = q.Where(x => x.StartTime >= query.StartTimeFrom.Value);
        if (query.StartTimeTo.HasValue) q = q.Where(x => x.StartTime <= query.StartTimeTo.Value);
        if (query.EndTimeFrom.HasValue) q = q.Where(x => x.EndTime >= query.EndTimeFrom.Value);
        if (query.EndTimeTo.HasValue) q = q.Where(x => x.EndTime <= query.EndTimeTo.Value);
        return ApplyOrdering(q, query.OrderBy, query.OrderDir);
    }

    private async Task<PagedResponse<ClassScheduleResponse>> ToPagedAsync(IQueryable<ClassSchedule> q, ClassScheduleListQuery query, CancellationToken cancellationToken)
    {
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<ClassScheduleResponse> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalItems = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }

    private async Task Validate(ClassScheduleRequest request, CancellationToken cancellationToken)
    {
        if (request.Weekday < 1 || request.Weekday > 7) throw new InvalidOperationException("weekday must be in range 1..7.");
        if (request.StartTime >= request.EndTime) throw new InvalidOperationException("start_time must be earlier than end_time.");
        var classExists = await _db.Set<ClassEntity>().AnyAsync(x => x.Id == request.ClassId, cancellationToken);
        if (!classExists) throw new InvalidOperationException("Class does not exist.");

        if (request.RoomId.HasValue)
        {
            var roomExists = await _db.Set<Room>().AnyAsync(x => x.Id == request.RoomId.Value, cancellationToken);
            if (!roomExists) throw new InvalidOperationException("Room does not exist.");
        }
    }

    private static IQueryable<ClassSchedule> ApplyOrdering(IQueryable<ClassSchedule> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "weekday" => asc ? q.OrderBy(x => x.Weekday) : q.OrderByDescending(x => x.Weekday),
            "starttime" => asc ? q.OrderBy(x => x.StartTime) : q.OrderByDescending(x => x.StartTime),
            "endtime" => asc ? q.OrderBy(x => x.EndTime) : q.OrderByDescending(x => x.EndTime),
            _ => asc ? q.OrderBy(x => x.CreatedAt) : q.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<ClassSchedule, ClassScheduleResponse>> Map() => x => new ClassScheduleResponse
    {
        Id = x.Id,
        ClassId = x.ClassId,
        RoomId = x.RoomId,
        Weekday = x.Weekday,
        StartTime = x.StartTime,
        EndTime = x.EndTime,
        CreatedAt = x.CreatedAt
    };
}
