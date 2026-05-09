using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Enrollments;

namespace app.Repositories.Enrollments;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _db;
    public EnrollmentRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<EnrollmentResponse>> GetAllAsync(EnrollmentListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Set<Enrollment>().AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var k = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x =>
                _db.Students
                    .Join(_db.Users, s => s.UserId, u => u.Id, (s, u) => new { s.Id, u.FullName })
                    .Any(su => su.Id == x.StudentId && su.FullName != null && su.FullName.ToLower().Contains(k))
                || _db.Classes.Any(c => c.Id == x.ClassId && c.Name != null && c.Name.ToLower().Contains(k)));
        }
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);
        if (query.EnrolledBy.HasValue) q = q.Where(x => x.EnrolledBy == query.EnrolledBy.Value);
        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => new EnrollmentResponse
            {
                Id = x.Id,
                StudentName = _db.Students
                    .Join(_db.Users, s => s.UserId, u => u.Id, (s, u) => new { s.Id, u.FullName })
                    .Where(su => su.Id == x.StudentId)
                    .Select(su => su.FullName)
                    .FirstOrDefault(),
                ClassName = _db.Classes.Where(c => c.Id == x.ClassId).Select(c => c.Name).FirstOrDefault(),
                Status = x.Status,
                TuitionFee = x.TuitionFee,
                Discount = x.Discount,
                DiscountReason = x.DiscountReason,
                FinalFee = x.FinalFee,
                EnrolledBy = x.EnrolledBy,
                EnrolledAt = x.EnrolledAt,
                CompletedAt = x.CompletedAt,
                Notes = x.Notes
            })
            .ToListAsync(cancellationToken);
        return new PagedResponse<EnrollmentResponse> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalItems = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }

    public async Task<EnrollmentResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Set<Enrollment>().AsNoTracking().Where(x => x.Id == id)
            .Select(x => new EnrollmentResponse
            {
                Id = x.Id,
                StudentName = _db.Students
                    .Join(_db.Users, s => s.UserId, u => u.Id, (s, u) => new { s.Id, u.FullName })
                    .Where(su => su.Id == x.StudentId)
                    .Select(su => su.FullName)
                    .FirstOrDefault(),
                ClassName = _db.Classes.Where(c => c.Id == x.ClassId).Select(c => c.Name).FirstOrDefault(),
                Status = x.Status,
                TuitionFee = x.TuitionFee,
                Discount = x.Discount,
                DiscountReason = x.DiscountReason,
                FinalFee = x.FinalFee,
                EnrolledBy = x.EnrolledBy,
                EnrolledAt = x.EnrolledAt,
                CompletedAt = x.CompletedAt,
                Notes = x.Notes
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<EnrollmentResponse> CreateAsync(EnrollmentRequest request, long enrolledByStaffId, CancellationToken cancellationToken = default)
    {
        await ValidateRefAsync(request, cancellationToken);

        var existed = await _db.Set<Enrollment>().AnyAsync(x => x.StudentId == request.StudentId && x.ClassId == request.ClassId, cancellationToken);
        if (existed) throw new InvalidOperationException("Enrollment already exists for this student and class.");

        var entity = new Enrollment
        {
            StudentId = request.StudentId,
            ClassId = request.ClassId,
            Status = request.Status ?? 1,
            TuitionFee = request.TuitionFee,
            Discount = request.Discount ?? 0,
            DiscountReason = request.DiscountReason,
            FinalFee = ComputeFinalFee(request.TuitionFee, request.Discount),
            EnrolledBy = enrolledByStaffId,
            CompletedAt = request.CompletedAt,
            Notes = request.Notes
        };

        _db.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(entity.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created enrollment.");
    }

    public async Task<EnrollmentResponse?> UpdateAsync(long id, EnrollmentRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Enrollment>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return null;

        await ValidateRefAsync(request, cancellationToken);
        var existed = await _db.Set<Enrollment>().AnyAsync(x => x.Id != id && x.StudentId == request.StudentId && x.ClassId == request.ClassId, cancellationToken);
        if (existed) throw new InvalidOperationException("Enrollment already exists for this student and class.");

        entity.StudentId = request.StudentId;
        entity.ClassId = request.ClassId;
        entity.Status = request.Status ?? entity.Status;
        entity.TuitionFee = request.TuitionFee;
        entity.Discount = request.Discount ?? entity.Discount;
        entity.DiscountReason = request.DiscountReason;
        entity.FinalFee = request.FinalFee ?? ComputeFinalFee(entity.TuitionFee, entity.Discount);
        entity.CompletedAt = request.CompletedAt;
        entity.Notes = request.Notes;

        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Set<Enrollment>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) throw new KeyNotFoundException("Enrollment not found.");
        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task ValidateRefAsync(EnrollmentRequest request, CancellationToken cancellationToken)
    {
        var studentExists = await _db.Set<Student>().AnyAsync(x => x.Id == request.StudentId, cancellationToken);
        if (!studentExists) throw new InvalidOperationException("Student does not exist.");

        var classExists = await _db.Set<ClassEntity>().AnyAsync(x => x.Id == request.ClassId, cancellationToken);
        if (!classExists) throw new InvalidOperationException("Class does not exist.");
    }

    private static decimal? ComputeFinalFee(decimal? tuitionFee, decimal? discount)
    {
        if (!tuitionFee.HasValue) return null;
        var d = discount ?? 0;
        return tuitionFee.Value - d;
    }

    private static IQueryable<Enrollment> ApplyOrdering(IQueryable<Enrollment> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "studentname" => asc
                ? q.OrderBy(x => x.StudentId)
                : q.OrderByDescending(x => x.StudentId),
            "classname" => asc
                ? q.OrderBy(x => x.ClassId)
                : q.OrderByDescending(x => x.ClassId),
            "status" => asc ? q.OrderBy(x => x.Status) : q.OrderByDescending(x => x.Status),
            _ => asc ? q.OrderBy(x => x.EnrolledAt) : q.OrderByDescending(x => x.EnrolledAt)
        };
    }

}
