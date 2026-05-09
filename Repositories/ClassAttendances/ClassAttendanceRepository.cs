using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.ClassAttendances;

namespace app.Repositories.ClassAttendances;

public class ClassAttendanceRepository : IClassAttendanceRepository
{
    private readonly AppDbContext _db;

    public ClassAttendanceRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResponse<ClassAttendanceResponse>> GetAllAsync(ClassAttendanceListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<ClassAttendance>().AsNoTracking().AsQueryable();

        if (query.ClassSessionId.HasValue) q = q.Where(x => x.ClassSessionId == query.ClassSessionId.Value);
        if (query.ClassStudentId.HasValue) q = q.Where(x => x.ClassStudentId == query.ClassStudentId.Value);
        if (query.IsAbsent.HasValue) q = q.Where(x => x.IsAbsent == query.IsAbsent.Value);
        if (query.RecordedBy.HasValue) q = q.Where(x => x.RecordedBy == query.RecordedBy.Value);
        if (query.RecordedFrom.HasValue) q = q.Where(x => x.RecordedAt >= query.RecordedFrom.Value);
        if (query.RecordedTo.HasValue) q = q.Where(x => x.RecordedAt <= query.RecordedTo.Value);

        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);

        var total = await q.LongCountAsync(cancellationToken);
        var items = await q
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(MapToResponse())
            .ToListAsync(cancellationToken);

        return new PagedResponse<ClassAttendanceResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = total,
            TotalPages = (int)Math.Ceiling(total / (double)query.PageSize)
        };
    }

    public async Task<ClassAttendanceResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.Set<ClassAttendance>()
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(MapToResponse())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ClassAttendanceResponse>> BulkCreateAsync(IReadOnlyCollection<ClassAttendanceRequest> requests, long recordedBy, CancellationToken cancellationToken = default)
    {
        if (requests.Count == 0) return Array.Empty<ClassAttendanceResponse>();

        var classSessionIds = requests.Select(x => x.ClassSessionId).Distinct().ToArray();
        var existingClassSessionIds = await _db.Set<ClassSession>()
            .AsNoTracking()
            .Where(x => classSessionIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingClassSessionIds = classSessionIds.Except(existingClassSessionIds).ToArray();
        if (missingClassSessionIds.Length > 0)
        {
            throw new InvalidOperationException($"Class session does not exist: {string.Join(", ", missingClassSessionIds)}");
        }

        var classStudentIds = requests.Select(x => x.ClassStudentId).Distinct().ToArray();
        var existingClassStudentIds = await _db.Set<ClassStudent>()
            .AsNoTracking()
            .Where(x => classStudentIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var missingClassStudentIds = classStudentIds.Except(existingClassStudentIds).ToArray();
        if (missingClassStudentIds.Length > 0)
        {
            throw new InvalidOperationException($"Class student does not exist: {string.Join(", ", missingClassStudentIds)}");
        }

        var entities = requests.Select(x => new ClassAttendance
        {
            ClassSessionId = x.ClassSessionId,
            ClassStudentId = x.ClassStudentId,
            IsAbsent = x.IsAbsent,
            AbsentReason = x.AbsentReason,
            RecordedBy = recordedBy
        }).ToList();

        await _db.Set<ClassAttendance>().AddRangeAsync(entities, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        var createdIds = entities.Select(x => x.Id).ToArray();
        var created = await _db.Set<ClassAttendance>()
            .AsNoTracking()
            .Where(x => createdIds.Contains(x.Id))
            .OrderBy(x => x.Id)
            .Select(MapToResponse())
            .ToListAsync(cancellationToken);

        return created;
    }

    public async Task<ClassAttendanceResponse?> UpdateAsync(long id, ClassAttendanceRequest request, long recordedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ClassAttendance>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        var classSessionExists = await _db.Set<ClassSession>().AnyAsync(x => x.Id == request.ClassSessionId, cancellationToken);
        if (!classSessionExists) throw new InvalidOperationException("Class session does not exist.");
        var classStudentExists = await _db.Set<ClassStudent>().AnyAsync(x => x.Id == request.ClassStudentId, cancellationToken);
        if (!classStudentExists) throw new InvalidOperationException("Class student does not exist.");

        entity.ClassSessionId = request.ClassSessionId;
        entity.ClassStudentId = request.ClassStudentId;
        entity.IsAbsent = request.IsAbsent;
        entity.AbsentReason = request.AbsentReason;
        entity.RecordedBy = recordedBy;
        entity.RecordedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<ClassAttendance>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Class attendance not found.");
        _db.Set<ClassAttendance>().Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<ClassAttendance> ApplyOrdering(IQueryable<ClassAttendance> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();

        return key switch
        {
            "class_session_id" => asc ? q.OrderBy(x => x.ClassSessionId) : q.OrderByDescending(x => x.ClassSessionId),
            "class_student_id" => asc ? q.OrderBy(x => x.ClassStudentId) : q.OrderByDescending(x => x.ClassStudentId),
            "is_absent" => asc ? q.OrderBy(x => x.IsAbsent) : q.OrderByDescending(x => x.IsAbsent),
            _ => asc ? q.OrderBy(x => x.RecordedAt) : q.OrderByDescending(x => x.RecordedAt)
        };
    }

    private static Expression<Func<ClassAttendance, ClassAttendanceResponse>> MapToResponse()
        => x => new ClassAttendanceResponse
        {
            Id = x.Id,
            ClassSessionId = x.ClassSessionId,
            ClassStudentId = x.ClassStudentId,
            IsAbsent = x.IsAbsent,
            AbsentReason = x.AbsentReason,
            RecordedBy = x.RecordedBy,
            RecordedAt = x.RecordedAt
        };
}
