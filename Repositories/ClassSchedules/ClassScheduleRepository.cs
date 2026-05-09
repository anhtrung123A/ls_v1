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
            TeacherId = request.TeacherId,
            RoomId = request.RoomId,
            Weekday = request.Weekday,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            OnlineLink = request.OnlineLink,
            Type = request.Type ?? 1,
            IsActive = request.IsActive ?? true
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
        entity.TeacherId = request.TeacherId;
        entity.RoomId = request.RoomId;
        entity.Weekday = request.Weekday;
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.OnlineLink = request.OnlineLink;
        entity.Type = request.Type ?? entity.Type;
        entity.IsActive = request.IsActive ?? entity.IsActive;
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
        if (query.TeacherId.HasValue) q = q.Where(x => x.TeacherId == query.TeacherId.Value);
        if (query.RoomId.HasValue) q = q.Where(x => x.RoomId == query.RoomId.Value);
        if (query.IsActive.HasValue) q = q.Where(x => x.IsActive == query.IsActive.Value);
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

        if (request.TeacherId.HasValue)
        {
            var teacherExists = await _db.Set<User>().AnyAsync(x => x.Id == request.TeacherId.Value, cancellationToken);
            if (!teacherExists) throw new InvalidOperationException("Teacher user does not exist.");
        }

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
            _ => asc ? q.OrderBy(x => x.CreatedAt) : q.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<ClassSchedule, ClassScheduleResponse>> Map() => x => new ClassScheduleResponse
    {
        Id = x.Id,
        ClassId = x.ClassId,
        TeacherId = x.TeacherId,
        RoomId = x.RoomId,
        Weekday = x.Weekday,
        StartTime = x.StartTime,
        EndTime = x.EndTime,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        OnlineLink = x.OnlineLink,
        Type = x.Type,
        IsActive = x.IsActive,
        CreatedAt = x.CreatedAt
    };
}
