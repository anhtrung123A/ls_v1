using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Analytics;

namespace app.Controllers;

[ApiController]
[Route("api/analytics/classes")]
[Authorize]
public class ClassAnalyticsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ClassAnalyticsController(AppDbContext db) { _db = db; }

    [HttpGet("overview")]
    public async Task<IResult> Overview(CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();

        var classQuery = _db.Classes.AsNoTracking();

        var totalClasses = await classQuery.LongCountAsync(cancellationToken);
        var upcomingClasses = await classQuery.LongCountAsync(x => x.Status == 1 || x.Status == 2, cancellationToken);
        var inProgressClasses = await classQuery.LongCountAsync(x => x.Status == 3, cancellationToken);
        var finishedClasses = await classQuery.LongCountAsync(x => x.Status == 4, cancellationToken);
        var cancelledClasses = await classQuery.LongCountAsync(x => x.Status == 5, cancellationToken);

        var totalCapacity = await classQuery.SumAsync(x => (long?)x.MaxStudents, cancellationToken) ?? 0L;
        var totalStudents = await classQuery.SumAsync(x => (long?)x.CurrentCount, cancellationToken) ?? 0L;

        var averageFillRate = totalCapacity == 0
            ? 0m
            : Math.Round(totalStudents * 100m / totalCapacity, 2);

        var result = new ClassOverviewResponse
        {
            TotalClasses = totalClasses,
            UpcomingClasses = upcomingClasses,
            InProgressClasses = inProgressClasses,
            FinishedClasses = finishedClasses,
            CancelledClasses = cancelledClasses,
            TotalCapacity = totalCapacity,
            TotalStudents = totalStudents,
            AverageFillRate = averageFillRate
        };

        return Results.Ok(ApiResponse.Ok(result, "Get class overview successfully."));
    }

    [HttpGet("fill-rate")]
    public async Task<IResult> FillRate([FromQuery] ClassFillRateQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();

        var classQuery = (
            from c in _db.Classes.AsNoTracking()
            join co in _db.Courses.AsNoTracking() on c.CourseId equals co.Id
            select new
            {
                c.Id,
                c.ClassCode,
                c.Name,
                CourseName = co.Name,
                c.MaxStudents,
                c.CurrentCount,
                c.Status
            }).AsQueryable();

        var statusCode = ParseClassStatus(query.Status);
        if (statusCode.HasValue)
        {
            classQuery = classQuery.Where(x => x.Status == statusCode.Value);
        }

        var rows = await classQuery
            .OrderByDescending(x => x.CurrentCount)
            .ThenBy(x => x.ClassCode)
            .ToListAsync(cancellationToken);

        var items = rows.Select(x =>
        {
            var availableSlots = Math.Max(0, x.MaxStudents - x.CurrentCount);
            var fillRate = x.MaxStudents <= 0
                ? 0m
                : Math.Round(x.CurrentCount * 100m / x.MaxStudents, 2);

            return new ClassFillRateItemResponse
            {
                ClassId = x.Id,
                ClassCode = x.ClassCode,
                ClassName = x.Name,
                CourseName = x.CourseName,
                MaxStudents = x.MaxStudents,
                CurrentCount = x.CurrentCount,
                AvailableSlots = availableSlots,
                FillRate = fillRate,
                Status = GetClassStatusName(x.Status)
            };
        }).ToList();

        var result = new ClassFillRateResponse
        {
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get class fill rate successfully."));
    }

    [HttpGet("by-course")]
    public async Task<IResult> ByCourse([FromQuery] ClassByCourseQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var classQuery = _db.Classes.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            classQuery = classQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            classQuery = classQuery.Where(x => x.CreatedAt < toExclusive);
        }

        var rows = await (
            from c in classQuery
            join co in _db.Courses.AsNoTracking() on c.CourseId equals co.Id
            select new
            {
                CourseId = co.Id,
                CourseCode = co.Code,
                CourseName = co.Name,
                c.MaxStudents,
                c.CurrentCount,
                c.Status
            }).ToListAsync(cancellationToken);

        var items = rows
            .GroupBy(x => new { x.CourseId, x.CourseCode, x.CourseName })
            .Select(g =>
            {
                var classCount = g.LongCount();
                var studentCount = g.Sum(x => (long)x.CurrentCount);
                var totalCapacity = g.Sum(x => (long)x.MaxStudents);

                return new ClassByCourseItemResponse
                {
                    CourseId = g.Key.CourseId,
                    CourseCode = g.Key.CourseCode,
                    CourseName = g.Key.CourseName,
                    ClassCount = classCount,
                    StudentCount = studentCount,
                    TotalCapacity = totalCapacity,
                    AverageFillRate = totalCapacity == 0
                        ? 0m
                        : Math.Round(studentCount * 100m / totalCapacity, 2),
                    CompletedClasses = g.LongCount(x => x.Status == 4),
                    InProgressClasses = g.LongCount(x => x.Status == 3),
                    UpcomingClasses = g.LongCount(x => x.Status == 1 || x.Status == 2)
                };
            })
            .OrderByDescending(x => x.ClassCount)
            .ThenByDescending(x => x.StudentCount)
            .ThenBy(x => x.CourseCode)
            .ToList();

        var result = new ClassByCourseResponse
        {
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get class analytics by course successfully."));
    }

    [HttpGet("sessions")]
    public async Task<IResult> Sessions([FromQuery] ClassSessionsOverviewQuery query, CancellationToken cancellationToken)
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
        var scheduledSessions = await sessionQuery.LongCountAsync(x => x.Status == 1, cancellationToken);
        var completedSessions = await sessionQuery.LongCountAsync(x => x.Status == 3, cancellationToken);
        var cancelledSessions = await sessionQuery.LongCountAsync(x => x.Status == 4, cancellationToken);
        var postponedSessions = await sessionQuery.LongCountAsync(x => x.Status == 5, cancellationToken);
        var rescheduledSessions = await sessionQuery.LongCountAsync(x => x.Status == 6, cancellationToken);

        var completionRate = totalSessions == 0
            ? 0m
            : Math.Round(completedSessions * 100m / totalSessions, 2);
        var cancelRate = totalSessions == 0
            ? 0m
            : Math.Round(cancelledSessions * 100m / totalSessions, 2);

        var result = new ClassSessionsOverviewResponse
        {
            TotalSessions = totalSessions,
            ScheduledSessions = scheduledSessions,
            CompletedSessions = completedSessions,
            CancelledSessions = cancelledSessions,
            PostponedSessions = postponedSessions,
            RescheduledSessions = rescheduledSessions,
            CompletionRate = completionRate,
            CancelRate = cancelRate
        };

        return Results.Ok(ApiResponse.Ok(result, "Get class sessions analytics successfully."));
    }

    [HttpGet("teacher-workload")]
    public async Task<IResult> TeacherWorkload([FromQuery] TeacherWorkloadQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var sessionQuery = _db.ClassSessions.AsNoTracking()
            .Where(x => x.TeacherId.HasValue);

        if (query.From.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate >= query.From.Value);
        }

        if (query.To.HasValue)
        {
            sessionQuery = sessionQuery.Where(x => x.SessionDate <= query.To.Value);
        }

        var rows = await (
            from s in sessionQuery
            join u in _db.Users.AsNoTracking() on s.TeacherId equals u.Id
            join a in _db.ClassAttendances.AsNoTracking() on s.Id equals a.ClassSessionId into attendances
            from a in attendances.DefaultIfEmpty()
            select new
            {
                SessionId = s.Id,
                s.ClassId,
                TeacherId = s.TeacherId!.Value,
                TeacherName = u.FullName,
                s.Status,
                s.StartTime,
                s.EndTime,
                IsAbsent = a != null ? a.IsAbsent : null
            })
            .ToListAsync(cancellationToken);

        var items = rows
            .GroupBy(x => new { x.TeacherId, x.TeacherName })
            .Select(g =>
            {
                var sessionRows = g
                    .GroupBy(x => x.SessionId)
                    .Select(x => x.First())
                    .ToList();

                var sessionCount = sessionRows.LongCount();
                var completedSessions = sessionRows.LongCount(x => x.Status == 3);
                var cancelledSessions = sessionRows.LongCount(x => x.Status == 4);
                var classCount = sessionRows.Select(x => x.ClassId).Distinct().LongCount();

                var totalMinutes = sessionRows.Sum(x => Math.Max(0d, (x.EndTime - x.StartTime).TotalMinutes));
                var totalHours = Math.Round((decimal)(totalMinutes / 60d), 2);

                var totalAttendanceRecords = g.LongCount(x => x.IsAbsent.HasValue);
                var presentCount = g.LongCount(x => x.IsAbsent == false);
                var averageAttendanceRate = totalAttendanceRecords == 0
                    ? 0m
                    : Math.Round(presentCount * 100m / totalAttendanceRecords, 2);

                return new TeacherWorkloadItemResponse
                {
                    TeacherId = g.Key.TeacherId,
                    TeacherName = g.Key.TeacherName,
                    ClassCount = classCount,
                    SessionCount = sessionCount,
                    TotalTeachingHours = totalHours,
                    CompletedSessions = completedSessions,
                    CancelledSessions = cancelledSessions,
                    AverageAttendanceRate = averageAttendanceRate
                };
            })
            .OrderByDescending(x => x.SessionCount)
            .ThenByDescending(x => x.TotalTeachingHours)
            .ThenBy(x => x.TeacherName)
            .ToList();

        var result = new TeacherWorkloadResponse
        {
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get class analytics teacher workload successfully."));
    }

    private void EnsureStaffOrAdminPermission()
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "1" && role != "2")
        {
            throw new UnauthorizedAccessException("Only admin or staff can view class analytics.");
        }
    }

    private static byte? ParseClassStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return null;
        }

        return status.Trim().ToLowerInvariant() switch
        {
            "upcoming" => 1,
            "schedule_created" => 2,
            "in_progress" => 3,
            "finished" => 4,
            "cancelled" => 5,
            _ => throw new InvalidOperationException("Invalid status. Supported: upcoming, schedule_created, in_progress, finished, cancelled.")
        };
    }

    private static string GetClassStatusName(byte status)
    {
        return status switch
        {
            1 => "upcoming",
            2 => "schedule_created",
            3 => "in_progress",
            4 => "finished",
            5 => "cancelled",
            _ => "unknown"
        };
    }

    private static void ValidateDateRange(DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            throw new InvalidOperationException("from must be less than or equal to to.");
        }
    }
}
