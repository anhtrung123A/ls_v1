using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Analytics;

namespace app.Controllers;

[ApiController]
[Route("api/analytics/attendance")]
[Authorize]
public class AttendanceAnalyticsController : ControllerBase
{
    private readonly AppDbContext _db;
    public AttendanceAnalyticsController(AppDbContext db) { _db = db; }

    [HttpGet("overview")]
    public async Task<IResult> Overview([FromQuery] AttendanceOverviewQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var sessionQuery = _db.ClassSessions.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate <= query.To.Value);
        }

        var totalSessions = await sessionQuery.LongCountAsync(cancellationToken);
        var completedSessions = await sessionQuery.LongCountAsync(x => x.Status == 3, cancellationToken);

        var attendanceRows = await (
            from a in _db.ClassAttendances.AsNoTracking()
            join s in sessionQuery on a.ClassSessionId equals s.Id
            select a.IsAbsent
        ).ToListAsync(cancellationToken);

        var totalAttendanceRecords = attendanceRows.LongCount();
        var absentCount = attendanceRows.LongCount(x => x == true);
        var presentCount = attendanceRows.LongCount(x => x == false);

        var attendanceRate = totalAttendanceRecords == 0
            ? 0m
            : Math.Round(presentCount * 100m / totalAttendanceRecords, 2);
        var absentRate = totalAttendanceRecords == 0
            ? 0m
            : Math.Round(absentCount * 100m / totalAttendanceRecords, 2);

        var result = new AttendanceOverviewResponse
        {
            TotalSessions = totalSessions,
            CompletedSessions = completedSessions,
            TotalAttendanceRecords = totalAttendanceRecords,
            PresentCount = presentCount,
            AbsentCount = absentCount,
            AttendanceRate = attendanceRate,
            AbsentRate = absentRate
        };

        return Results.Ok(ApiResponse.Ok(result, "Get attendance overview successfully."));
    }

    [HttpGet("by-class")]
    public async Task<IResult> ByClass([FromQuery] AttendanceOverviewQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var sessionQuery = _db.ClassSessions.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate <= query.To.Value);
        }

        var rows = await (
            from a in _db.ClassAttendances.AsNoTracking()
            join s in sessionQuery on a.ClassSessionId equals s.Id
            join c in _db.Classes.AsNoTracking() on s.ClassId equals c.Id
            select new
            {
                c.Id,
                c.ClassCode,
                c.Name,
                a.IsAbsent
            })
            .ToListAsync(cancellationToken);

        var items = rows
            .GroupBy(x => new { x.Id, x.ClassCode, x.Name })
            .Select(g =>
            {
                var total = g.LongCount();
                var absent = g.LongCount(x => x.IsAbsent == true);
                var present = g.LongCount(x => x.IsAbsent == false);
                return new AttendanceByClassItemResponse
                {
                    ClassId = g.Key.Id,
                    ClassCode = g.Key.ClassCode,
                    ClassName = g.Key.Name,
                    TotalRecords = total,
                    PresentCount = present,
                    AbsentCount = absent,
                    AttendanceRate = total == 0 ? 0m : Math.Round(present * 100m / total, 2),
                    AbsentRate = total == 0 ? 0m : Math.Round(absent * 100m / total, 2)
                };
            })
            .OrderByDescending(x => x.AttendanceRate)
            .ThenBy(x => x.ClassCode)
            .ToList();

        var result = new AttendanceByClassResponse
        {
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get attendance analytics by class successfully."));
    }

    [HttpGet("by-student")]
    public async Task<IResult> ByStudent([FromQuery] AttendanceOverviewQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var sessionQuery = _db.ClassSessions.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate <= query.To.Value);
        }

        var rows = await (
            from a in _db.ClassAttendances.AsNoTracking()
            join s in sessionQuery on a.ClassSessionId equals s.Id
            join cs in _db.ClassStudents.AsNoTracking() on a.ClassStudentId equals cs.Id
            join st in _db.Students.AsNoTracking() on cs.StudentId equals st.Id
            join u in _db.Users.AsNoTracking() on st.UserId equals u.Id into users
            from u in users.DefaultIfEmpty()
            select new
            {
                StudentId = st.Id,
                st.StudentCode,
                StudentName = u != null ? u.FullName : null,
                cs.ClassId,
                a.IsAbsent
            })
            .ToListAsync(cancellationToken);

        var items = rows
            .GroupBy(x => new { x.StudentId, x.StudentCode, x.StudentName })
            .Select(g =>
            {
                var total = g.LongCount();
                var absent = g.LongCount(x => x.IsAbsent == true);
                var present = g.LongCount(x => x.IsAbsent == false);
                return new AttendanceByStudentItemResponse
                {
                    StudentId = g.Key.StudentId,
                    StudentCode = g.Key.StudentCode,
                    StudentName = g.Key.StudentName,
                    ClassCount = g.Select(x => x.ClassId).Distinct().LongCount(),
                    TotalRecords = total,
                    PresentCount = present,
                    AbsentCount = absent,
                    AttendanceRate = total == 0 ? 0m : Math.Round(present * 100m / total, 2),
                    AbsentRate = total == 0 ? 0m : Math.Round(absent * 100m / total, 2)
                };
            })
            .OrderBy(x => x.AttendanceRate)
            .ThenBy(x => x.StudentCode)
            .ThenBy(x => x.StudentId)
            .ToList();

        var result = new AttendanceByStudentResponse
        {
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get attendance analytics by student successfully."));
    }

    [HttpGet("risk-students")]
    public async Task<IResult> RiskStudents([FromQuery] AttendanceRiskStudentsQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var threshold = query.Threshold <= 0 ? 70m : query.Threshold;
        if (threshold > 100m)
        {
            threshold = 100m;
        }

        var sessionQuery = _db.ClassSessions.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate <= query.To.Value);
        }

        var rows = await (
            from a in _db.ClassAttendances.AsNoTracking()
            join s in sessionQuery on a.ClassSessionId equals s.Id
            join cs in _db.ClassStudents.AsNoTracking() on a.ClassStudentId equals cs.Id
            join st in _db.Students.AsNoTracking() on cs.StudentId equals st.Id
            join c in _db.Classes.AsNoTracking() on cs.ClassId equals c.Id
            join u in _db.Users.AsNoTracking() on st.UserId equals u.Id into users
            from u in users.DefaultIfEmpty()
            select new
            {
                StudentId = st.Id,
                st.StudentCode,
                StudentName = u != null ? u.FullName : null,
                ClassId = c.Id,
                ClassName = c.Name,
                a.IsAbsent,
                s.SessionDate
            })
            .ToListAsync(cancellationToken);

        var items = rows
            .GroupBy(x => new { x.StudentId, x.StudentCode, x.StudentName, x.ClassId, x.ClassName })
            .Select(g =>
            {
                var total = g.LongCount();
                var absent = g.LongCount(x => x.IsAbsent == true);
                var present = g.LongCount(x => x.IsAbsent == false);
                var attendanceRate = total == 0 ? 0m : Math.Round(present * 100m / total, 2);
                var absentRate = total == 0 ? 0m : Math.Round(absent * 100m / total, 2);
                var lastAbsentDate = g
                    .Where(x => x.IsAbsent == true)
                    .OrderByDescending(x => x.SessionDate)
                    .Select(x => (DateOnly?)x.SessionDate)
                    .FirstOrDefault();

                return new AttendanceRiskStudentItemResponse
                {
                    StudentId = g.Key.StudentId,
                    StudentCode = g.Key.StudentCode,
                    StudentName = g.Key.StudentName,
                    ClassId = g.Key.ClassId,
                    ClassName = g.Key.ClassName,
                    TotalRecords = total,
                    PresentCount = present,
                    AbsentCount = absent,
                    AttendanceRate = attendanceRate,
                    AbsentRate = absentRate,
                    LastAbsentDate = lastAbsentDate,
                    RiskLevel = GetRiskLevel(attendanceRate)
                };
            })
            .Where(x => x.AttendanceRate < threshold)
            .OrderBy(x => x.AttendanceRate)
            .ThenByDescending(x => x.AbsentCount)
            .ThenBy(x => x.StudentCode)
            .ThenBy(x => x.ClassId)
            .ToList();

        var result = new AttendanceRiskStudentsResponse
        {
            Threshold = threshold,
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get risk students by attendance successfully."));
    }

    private void EnsureStaffOrAdminPermission()
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "1" && role != "2")
        {
            throw new UnauthorizedAccessException("Only admin or staff can view attendance analytics.");
        }
    }

    private static void ValidateDateRange(DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            throw new InvalidOperationException("from must be less than or equal to to.");
        }
    }

    private static string GetRiskLevel(decimal attendanceRate)
    {
        if (attendanceRate < 50m)
        {
            return "high";
        }

        return "medium";
    }
}
