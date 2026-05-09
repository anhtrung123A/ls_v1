using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Classes;

namespace app.Repositories.Classes;

public class ClassRepository : IClassRepository
{
    private readonly AppDbContext _db;
    public ClassRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<ClassResponse>> GetAllAsync(ClassListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<ClassEntity>().AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var k = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x => x.ClassCode.ToLower().Contains(k) || (x.Name != null && x.Name.ToLower().Contains(k)));
        }
        if (query.CourseId.HasValue) q = q.Where(x => x.CourseId == query.CourseId.Value);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);
        if (query.Type.HasValue) q = q.Where(x => x.Type == query.Type.Value);
        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<ClassResponse> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalItems = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }

    public async Task<ClassResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Set<ClassEntity>().AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);

    public async Task<ClassResponse> CreateAsync(ClassRequest request, CancellationToken cancellationToken = default)
    {
        await ValidateRefs(request, cancellationToken);
        var code = string.IsNullOrWhiteSpace(request.ClassCode) ? null : request.ClassCode.Trim().ToUpperInvariant();

        var entity = new ClassEntity
        {
            CourseId = request.CourseId,
            ClassCode = code ?? "TEMP",
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            MaxStudents = request.MaxStudents ?? 20,
            CurrentCount = request.CurrentCount ?? 0,
            Type = request.Type ?? 1,
            Status = request.Status ?? 1,
            TeacherId = request.TeacherId,
            CreatedBy = request.CreatedBy
        };

        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        entity.ClassCode = code ?? $"CLS{entity.Id:D6}";
        var existed = await _db.Set<ClassEntity>().AnyAsync(x => x.Id != entity.Id && x.ClassCode == entity.ClassCode, cancellationToken);
        if (existed) throw new InvalidOperationException("Class code already exists.");

        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created class.");
    }

    public async Task<ClassResponse?> UpdateAsync(long id, ClassRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ClassEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;
        await ValidateRefs(request, cancellationToken);

        var code = string.IsNullOrWhiteSpace(request.ClassCode) ? entity.ClassCode : request.ClassCode.Trim().ToUpperInvariant();
        var existed = await _db.Set<ClassEntity>().AnyAsync(x => x.Id != id && x.ClassCode == code, cancellationToken);
        if (existed) throw new InvalidOperationException("Class code already exists.");

        entity.CourseId = request.CourseId;
        entity.ClassCode = code;
        entity.Name = request.Name;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.MaxStudents = request.MaxStudents ?? entity.MaxStudents;
        entity.CurrentCount = request.CurrentCount ?? entity.CurrentCount;
        entity.Type = request.Type ?? entity.Type;
        entity.Status = request.Status ?? entity.Status;
        entity.TeacherId = request.TeacherId;
        entity.CreatedBy = request.CreatedBy;
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<ClassResponse> SetScheduleCreatedAsync(long id, long actorUserId, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ClassEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Class not found.");
        if (!entity.StartDate.HasValue || !entity.EndDate.HasValue) throw new InvalidOperationException("Class must have start_date and end_date.");
        if (entity.StartDate > entity.EndDate) throw new InvalidOperationException("Class start_date must be earlier than or equal to end_date.");

        var schedules = await _db.Set<ClassSchedule>()
            .AsNoTracking()
            .Where(x => x.ClassId == id)
            .ToListAsync(cancellationToken);

        if (schedules.Count == 0) throw new InvalidOperationException("Class must have at least one class_schedule.");

        var sessionExists = await _db.Set<ClassSession>().AnyAsync(x => x.ClassId == id, cancellationToken);
        if (sessionExists) throw new InvalidOperationException("Class sessions already created for this class.");

        var sessions = new List<ClassSession>();
        for (var date = entity.StartDate.Value; date <= entity.EndDate.Value; date = date.AddDays(1))
        {
            var weekday = ToDbWeekday(date.DayOfWeek);
            foreach (var schedule in schedules.Where(s => s.Weekday == weekday))
            {
                sessions.Add(new ClassSession
                {
                    ClassId = id,
                    SessionDate = date,
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    TeacherId = entity.TeacherId,
                    RoomId = schedule.RoomId,
                    Type = entity.Type == 2 ? (byte)2 : (byte)1,
                    Status = 1,
                    CreatedBy = actorUserId,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        if (sessions.Count == 0) throw new InvalidOperationException("No class_session can be generated from current class_schedules.");

        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        _db.Set<ClassSession>().AddRange(sessions);
        entity.Status = 2; // schedule_created
        await _db.SaveChangesAsync(cancellationToken);
        await tx.CommitAsync(cancellationToken);

        return await GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Class not found after update.");
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ClassEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Class not found.");
        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateRefs(ClassRequest request, CancellationToken cancellationToken)
    {
        var courseExists = await _db.Set<Course>().AnyAsync(x => x.Id == request.CourseId, cancellationToken);
        if (!courseExists) throw new InvalidOperationException("Course does not exist.");

        if (request.CreatedBy.HasValue)
        {
            var userExists = await _db.Set<User>().AnyAsync(x => x.Id == request.CreatedBy.Value, cancellationToken);
            if (!userExists) throw new InvalidOperationException("CreatedBy user does not exist.");
        }

        if (request.TeacherId.HasValue)
        {
            var teacherExists = await _db.Set<User>().AnyAsync(x => x.Id == request.TeacherId.Value, cancellationToken);
            if (!teacherExists) throw new InvalidOperationException("Teacher user does not exist.");
        }
    }

    private static IQueryable<ClassEntity> ApplyOrdering(IQueryable<ClassEntity> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "classcode" => asc ? q.OrderBy(x => x.ClassCode) : q.OrderByDescending(x => x.ClassCode),
            "name" => asc ? q.OrderBy(x => x.Name) : q.OrderByDescending(x => x.Name),
            "startdate" => asc ? q.OrderBy(x => x.StartDate) : q.OrderByDescending(x => x.StartDate),
            "status" => asc ? q.OrderBy(x => x.Status) : q.OrderByDescending(x => x.Status),
            _ => asc ? q.OrderBy(x => x.CreatedAt) : q.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<ClassEntity, ClassResponse>> Map() => x => new ClassResponse
    {
        Id = x.Id,
        CourseId = x.CourseId,
        ClassCode = x.ClassCode,
        Name = x.Name,
        StartDate = x.StartDate,
        EndDate = x.EndDate,
        MaxStudents = x.MaxStudents,
        CurrentCount = x.CurrentCount,
        Type = x.Type,
        Status = x.Status,
        TeacherId = x.TeacherId,
        CreatedBy = x.CreatedBy,
        CreatedAt = x.CreatedAt
    };

    private static byte ToDbWeekday(DayOfWeek dayOfWeek)
    {
        var day = (int)dayOfWeek;
        return (byte)(day == 0 ? 7 : day);
    }
}
