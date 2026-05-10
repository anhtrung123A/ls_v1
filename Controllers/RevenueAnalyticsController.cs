using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Analytics;

namespace app.Controllers;

[ApiController]
[Route("api/analytics/revenue")]
[Authorize]
public class RevenueAnalyticsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RevenueAnalyticsController(AppDbContext db) { _db = db; }

    [HttpGet("overview")]
    public async Task<IResult> Overview(CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();

        var invoices = _db.Invoices.AsNoTracking();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var totalRevenue = await invoices.SumAsync(x => (decimal?)x.FinalAmount, cancellationToken) ?? 0m;
        var paidRevenue = await invoices.SumAsync(x => (decimal?)x.PaidAmount, cancellationToken) ?? 0m;
        var outstandingRevenue = await invoices.SumAsync(x => (decimal?)x.RemainingAmount, cancellationToken) ?? 0m;

        var totalInvoices = await invoices.LongCountAsync(cancellationToken);
        var paidInvoices = await invoices.LongCountAsync(x => x.Status == 3, cancellationToken);
        var overdueInvoices = await invoices.LongCountAsync(x => x.Status == 4 || (x.DueDate.HasValue && x.DueDate.Value < today && x.Status != 3), cancellationToken);

        var result = new RevenueOverviewResponse
        {
            TotalRevenue = totalRevenue,
            PaidRevenue = paidRevenue,
            OutstandingRevenue = outstandingRevenue,
            TotalInvoices = totalInvoices,
            PaidInvoices = paidInvoices,
            OverdueInvoices = overdueInvoices
        };

        return Results.Ok(ApiResponse.Ok(result, "Get revenue overview successfully."));
    }

    [HttpGet("trend")]
    public async Task<IResult> Trend([FromQuery] RevenueTrendQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var groupBy = string.Equals(query.GroupBy, "day", StringComparison.OrdinalIgnoreCase) ? "day" : "month";
        var invoiceQuery = _db.Invoices.AsNoTracking().AsQueryable();

        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            invoiceQuery = invoiceQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            invoiceQuery = invoiceQuery.Where(x => x.CreatedAt < toExclusive);
        }

        List<RevenueTrendItemResponse> items;
        if (groupBy == "day")
        {
            var grouped = await invoiceQuery
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.FinalAmount),
                    PaidRevenue = g.Sum(x => x.PaidAmount),
                    OutstandingRevenue = g.Sum(x => x.RemainingAmount),
                    InvoiceCount = g.LongCount()
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            items = grouped.Select(x => new RevenueTrendItemResponse
            {
                Period = x.Date.ToString("yyyy-MM-dd"),
                Revenue = x.Revenue,
                PaidRevenue = x.PaidRevenue,
                OutstandingRevenue = x.OutstandingRevenue,
                InvoiceCount = x.InvoiceCount
            }).ToList();
        }
        else
        {
            var grouped = await invoiceQuery
                .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    Revenue = g.Sum(x => x.FinalAmount),
                    PaidRevenue = g.Sum(x => x.PaidAmount),
                    OutstandingRevenue = g.Sum(x => x.RemainingAmount),
                    InvoiceCount = g.LongCount()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(cancellationToken);

            items = grouped.Select(x => new RevenueTrendItemResponse
            {
                Period = $"{x.Year:D4}-{x.Month:D2}",
                Revenue = x.Revenue,
                PaidRevenue = x.PaidRevenue,
                OutstandingRevenue = x.OutstandingRevenue,
                InvoiceCount = x.InvoiceCount
            }).ToList();
        }

        var result = new RevenueTrendResponse
        {
            GroupBy = groupBy,
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get revenue trend successfully."));
    }

    [HttpGet("by-course")]
    public async Task<IResult> ByCourse([FromQuery] RevenueTrendQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var invoiceQuery = _db.Invoices.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            invoiceQuery = invoiceQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            invoiceQuery = invoiceQuery.Where(x => x.CreatedAt < toExclusive);
        }

        var rows = await (
            from i in invoiceQuery
            join e in _db.Enrollments.AsNoTracking() on i.EnrollmentId equals e.Id
            join c in _db.Classes.AsNoTracking() on e.ClassId equals c.Id
            join co in _db.Courses.AsNoTracking() on c.CourseId equals co.Id
            select new
            {
                CourseId = co.Id,
                CourseName = co.Name,
                InvoiceId = i.Id,
                StudentId = e.StudentId,
                i.FinalAmount,
                i.PaidAmount,
                i.RemainingAmount
            })
            .ToListAsync(cancellationToken);

        var items = rows
            .GroupBy(x => new { x.CourseId, x.CourseName })
            .Select(g => new RevenueByCourseItemResponse
            {
                CourseId = g.Key.CourseId,
                CourseName = g.Key.CourseName,
                InvoiceCount = g.Select(x => x.InvoiceId).Distinct().LongCount(),
                StudentCount = g.Select(x => x.StudentId).Distinct().LongCount(),
                TotalRevenue = g.Sum(x => x.FinalAmount),
                PaidRevenue = g.Sum(x => x.PaidAmount),
                OutstandingRevenue = g.Sum(x => x.RemainingAmount)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToList();

        var result = new RevenueByCourseResponse
        {
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get revenue analytics by course successfully."));
    }

    [HttpGet("payment-methods")]
    public async Task<IResult> PaymentMethods([FromQuery] RevenueTrendQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var paymentQuery = _db.Payments.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            paymentQuery = paymentQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            paymentQuery = paymentQuery.Where(x => x.CreatedAt < toExclusive);
        }

        var grouped = await paymentQuery
            .GroupBy(x => x.Method)
            .Select(g => new
            {
                Method = g.Key,
                PaymentCount = g.LongCount(),
                TotalAmount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalAmount)
            .ToListAsync(cancellationToken);

        var totalAmount = grouped.Sum(x => x.TotalAmount);
        var items = grouped.Select(x => new RevenuePaymentMethodItemResponse
        {
            Method = x.Method,
            MethodName = GetPaymentMethodName(x.Method),
            PaymentCount = x.PaymentCount,
            TotalAmount = x.TotalAmount,
            Percentage = totalAmount == 0 ? 0m : Math.Round(x.TotalAmount * 100m / totalAmount, 2)
        }).ToList();

        var result = new RevenuePaymentMethodsResponse
        {
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get revenue analytics by payment method successfully."));
    }

    [HttpGet("outstanding")]
    public async Task<IResult> Outstanding([FromQuery] RevenueOutstandingQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var invoiceQuery = _db.Invoices.AsNoTracking()
            .Where(x => x.RemainingAmount > 0 && (x.Status == 1 || x.Status == 2 || x.Status == 4));

        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            invoiceQuery = invoiceQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            invoiceQuery = invoiceQuery.Where(x => x.CreatedAt < toExclusive);
        }

        var rows = await (
            from i in invoiceQuery
            join e in _db.Enrollments.AsNoTracking() on i.EnrollmentId equals e.Id
            join c in _db.Classes.AsNoTracking() on e.ClassId equals c.Id
            join co in _db.Courses.AsNoTracking() on c.CourseId equals co.Id
            join s in _db.Students.AsNoTracking() on i.StudentId equals s.Id
            join u in _db.Users.AsNoTracking() on s.UserId equals u.Id into users
            from u in users.DefaultIfEmpty()
            select new
            {
                i.Id,
                i.InvoiceNo,
                i.StudentId,
                StudentName = u != null ? u.FullName : null,
                CourseName = co.Name,
                i.FinalAmount,
                i.PaidAmount,
                i.RemainingAmount,
                i.DueDate,
                i.Status
            })
            .OrderByDescending(x => x.DueDate)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        var items = rows.Select(x =>
        {
            var overdueDays = x.DueDate.HasValue ? Math.Max(0, (today.DayNumber - x.DueDate.Value.DayNumber)) : 0;
            return new RevenueOutstandingItemResponse
            {
                InvoiceId = x.Id,
                InvoiceNo = x.InvoiceNo,
                StudentId = x.StudentId,
                StudentName = x.StudentName,
                CourseName = x.CourseName,
                FinalAmount = x.FinalAmount,
                PaidAmount = x.PaidAmount,
                RemainingAmount = x.RemainingAmount,
                DueDate = x.DueDate,
                DaysOverdue = overdueDays,
                Status = GetOutstandingStatusName(x.Status, overdueDays)
            };
        }).ToList();

        var totalOutstandingAmount = items.Sum(x => x.RemainingAmount);
        var totalOutstandingInvoices = items.LongCount();
        var totalOverdueInvoices = items.LongCount(x => x.DaysOverdue > 0);

        var aging = new List<RevenueOutstandingAgingItemResponse>
        {
            new()
            {
                Bucket = "0-7",
                Amount = items.Where(x => x.DaysOverdue <= 7).Sum(x => x.RemainingAmount)
            },
            new()
            {
                Bucket = "8-30",
                Amount = items.Where(x => x.DaysOverdue >= 8 && x.DaysOverdue <= 30).Sum(x => x.RemainingAmount)
            },
            new()
            {
                Bucket = ">30",
                Amount = items.Where(x => x.DaysOverdue > 30).Sum(x => x.RemainingAmount)
            }
        };

        var result = new RevenueOutstandingResponse
        {
            TotalOutstandingAmount = totalOutstandingAmount,
            TotalOutstandingInvoices = totalOutstandingInvoices,
            TotalOverdueInvoices = totalOverdueInvoices,
            Items = items,
            Aging = aging
        };

        return Results.Ok(ApiResponse.Ok(result, "Get outstanding revenue successfully."));
    }

    private void EnsureStaffOrAdminPermission()
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "1" && role != "2")
        {
            throw new UnauthorizedAccessException("Only admin or staff can view revenue analytics.");
        }
    }

    private static void ValidateDateRange(DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            throw new InvalidOperationException("from must be less than or equal to to.");
        }
    }

    private static string GetPaymentMethodName(byte? method)
    {
        return method switch
        {
            1 => "Cash",
            2 => "Bank Transfer",
            3 => "Card",
            4 => "MoMo",
            5 => "VNPay",
            _ => "Unknown"
        };
    }

    private static string GetOutstandingStatusName(byte status, int daysOverdue)
    {
        if (status == 4 || daysOverdue > 0)
        {
            return "overdue";
        }

        return status switch
        {
            1 => "unpaid",
            2 => "partial",
            _ => "unpaid"
        };
    }
}
