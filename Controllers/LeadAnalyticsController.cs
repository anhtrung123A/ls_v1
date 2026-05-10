using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Analytics;

namespace app.Controllers;

[ApiController]
[Route("api/analytics/leads")]
[Authorize]
public class LeadAnalyticsController : ControllerBase
{
    private readonly AppDbContext _db;
    public LeadAnalyticsController(AppDbContext db) { _db = db; }

    [HttpGet("overview")]
    public async Task<IResult> Overview([FromQuery] LeadOverviewQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var leadQuery = _db.Leads.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt < toExclusive);
        }

        var totalLeads = await leadQuery.LongCountAsync(cancellationToken);
        var newLeads = await leadQuery.LongCountAsync(x => x.Status == 1, cancellationToken);
        var contactedLeads = await leadQuery.LongCountAsync(x => x.Status == 2, cancellationToken);
        var qualifiedLeads = await leadQuery.LongCountAsync(x => x.Status == 3, cancellationToken);
        var demoScheduledLeads = await leadQuery.LongCountAsync(x => x.Status == 4, cancellationToken);
        var convertedLeads = await leadQuery.LongCountAsync(x => x.Status == 5, cancellationToken);
        var lostLeads = await leadQuery.LongCountAsync(x => x.Status == 6, cancellationToken);

        var conversionRate = totalLeads == 0 ? 0m : Math.Round((decimal)convertedLeads * 100m / totalLeads, 2);
        var lostRate = totalLeads == 0 ? 0m : Math.Round((decimal)lostLeads * 100m / totalLeads, 2);

        var result = new LeadOverviewResponse
        {
            TotalLeads = totalLeads,
            NewLeads = newLeads,
            ContactedLeads = contactedLeads,
            QualifiedLeads = qualifiedLeads,
            DemoScheduledLeads = demoScheduledLeads,
            ConvertedLeads = convertedLeads,
            LostLeads = lostLeads,
            ConversionRate = conversionRate,
            LostRate = lostRate
        };

        return Results.Ok(ApiResponse.Ok(result, "Get lead overview successfully."));
    }

    [HttpGet("by-source")]
    public async Task<IResult> BySource([FromQuery] LeadOverviewQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var leadQuery = _db.Leads.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt < toExclusive);
        }

        var grouped = await leadQuery
            .GroupBy(x => x.Source)
            .Select(g => new
            {
                Source = g.Key,
                TotalLeads = g.LongCount(),
                ConvertedLeads = g.LongCount(x => x.Status == 5),
                LostLeads = g.LongCount(x => x.Status == 6)
            })
            .OrderByDescending(x => x.TotalLeads)
            .ToListAsync(cancellationToken);

        var items = grouped.Select(x =>
        {
            var conversionRate = x.TotalLeads == 0 ? 0m : Math.Round((decimal)x.ConvertedLeads * 100m / x.TotalLeads, 2);
            var lostRate = x.TotalLeads == 0 ? 0m : Math.Round((decimal)x.LostLeads * 100m / x.TotalLeads, 2);

            return new LeadBySourceItemResponse
            {
                Source = x.Source,
                SourceName = GetSourceName(x.Source),
                TotalLeads = x.TotalLeads,
                ConvertedLeads = x.ConvertedLeads,
                LostLeads = x.LostLeads,
                ConversionRate = conversionRate,
                LostRate = lostRate
            };
        }).ToList();

        var result = new LeadBySourceResponse { Items = items };
        return Results.Ok(ApiResponse.Ok(result, "Get lead analytics by source successfully."));
    }

    [HttpGet("by-staff")]
    public async Task<IResult> ByStaff([FromQuery] LeadOverviewQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var leadQuery = _db.Leads.AsNoTracking().Where(x => x.AssignedTo.HasValue);
        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt < toExclusive);
        }

        var grouped = await (
            from l in leadQuery
            join s in _db.Staff.AsNoTracking() on l.AssignedTo!.Value equals s.Id
            join u in _db.Users.AsNoTracking() on s.UserId equals u.Id
            group l by new { s.Id, u.FullName } into g
            select new
            {
                StaffId = g.Key.Id,
                StaffName = g.Key.FullName,
                AssignedLeads = g.LongCount(),
                ContactedLeads = g.LongCount(x => x.Status == 2),
                QualifiedLeads = g.LongCount(x => x.Status == 3),
                DemoScheduledLeads = g.LongCount(x => x.Status == 4),
                ConvertedLeads = g.LongCount(x => x.Status == 5),
                LostLeads = g.LongCount(x => x.Status == 6)
            })
            .OrderByDescending(x => x.AssignedLeads)
            .ToListAsync(cancellationToken);

        var items = grouped.Select(x =>
        {
            var conversionRate = x.AssignedLeads == 0 ? 0m : Math.Round((decimal)x.ConvertedLeads * 100m / x.AssignedLeads, 2);
            var lostRate = x.AssignedLeads == 0 ? 0m : Math.Round((decimal)x.LostLeads * 100m / x.AssignedLeads, 2);
            return new LeadByStaffItemResponse
            {
                StaffId = x.StaffId,
                StaffName = x.StaffName ?? string.Empty,
                AssignedLeads = x.AssignedLeads,
                ContactedLeads = x.ContactedLeads,
                QualifiedLeads = x.QualifiedLeads,
                DemoScheduledLeads = x.DemoScheduledLeads,
                ConvertedLeads = x.ConvertedLeads,
                LostLeads = x.LostLeads,
                ConversionRate = conversionRate,
                LostRate = lostRate
            };
        }).ToList();

        var result = new LeadByStaffResponse { Items = items };
        return Results.Ok(ApiResponse.Ok(result, "Get lead analytics by staff successfully."));
    }

    [HttpGet("trend")]
    public async Task<IResult> Trend([FromQuery] LeadTrendQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var groupBy = string.Equals(query.GroupBy, "month", StringComparison.OrdinalIgnoreCase) ? "month" : "day";

        var leadQuery = _db.Leads.AsNoTracking().AsQueryable();
        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt < toExclusive);
        }

        List<LeadTrendItemResponse> items;
        if (groupBy == "month")
        {
            var grouped = await leadQuery
                .GroupBy(x => new { x.CreatedAt.Year, x.CreatedAt.Month })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    TotalLeads = g.LongCount(),
                    ConvertedLeads = g.LongCount(x => x.Status == 5),
                    LostLeads = g.LongCount(x => x.Status == 6)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(cancellationToken);

            items = grouped.Select(x => new LeadTrendItemResponse
            {
                Date = $"{x.Year:D4}-{x.Month:D2}",
                TotalLeads = x.TotalLeads,
                ConvertedLeads = x.ConvertedLeads,
                LostLeads = x.LostLeads,
                ConversionRate = x.TotalLeads == 0 ? 0m : Math.Round((decimal)x.ConvertedLeads * 100m / x.TotalLeads, 2)
            }).ToList();
        }
        else
        {
            var grouped = await leadQuery
                .GroupBy(x => x.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalLeads = g.LongCount(),
                    ConvertedLeads = g.LongCount(x => x.Status == 5),
                    LostLeads = g.LongCount(x => x.Status == 6)
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            items = grouped.Select(x => new LeadTrendItemResponse
            {
                Date = x.Date.ToString("yyyy-MM-dd"),
                TotalLeads = x.TotalLeads,
                ConvertedLeads = x.ConvertedLeads,
                LostLeads = x.LostLeads,
                ConversionRate = x.TotalLeads == 0 ? 0m : Math.Round((decimal)x.ConvertedLeads * 100m / x.TotalLeads, 2)
            }).ToList();
        }

        var result = new LeadTrendResponse
        {
            GroupBy = groupBy,
            Items = items
        };

        return Results.Ok(ApiResponse.Ok(result, "Get lead trend successfully."));
    }

    [HttpGet("aging")]
    public async Task<IResult> Aging([FromQuery] LeadOverviewQuery query, CancellationToken cancellationToken)
    {
        EnsureStaffOrAdminPermission();
        ValidateDateRange(query.From, query.To);

        var leadQuery = _db.Leads
            .AsNoTracking()
            .Where(x => x.Status != 5 && x.Status != 6) // open leads only
            .AsQueryable();

        if (query.From.HasValue)
        {
            var from = query.From.Value.ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt >= from);
        }

        if (query.To.HasValue)
        {
            var toExclusive = query.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue);
            leadQuery = leadQuery.Where(x => x.CreatedAt < toExclusive);
        }

        var nowDate = DateTime.UtcNow.Date;
        var dayValues = await leadQuery
            .Select(x => EF.Functions.DateDiffDay(x.UpdatedAt.Date, nowDate))
            .ToListAsync(cancellationToken);

        long bucket0To1 = 0;
        long bucket2To7 = 0;
        long bucket8To14 = 0;
        long bucket15To30 = 0;
        long bucketOver30 = 0;

        foreach (var days in dayValues)
        {
            var d = days < 0 ? 0 : days;
            if (d <= 1) bucket0To1++;
            else if (d <= 7) bucket2To7++;
            else if (d <= 14) bucket8To14++;
            else if (d <= 30) bucket15To30++;
            else bucketOver30++;
        }

        var items = new List<LeadAgingItemResponse>
        {
            new() { Bucket = "0-1 ngày", Count = bucket0To1 },
            new() { Bucket = "2-7 ngày", Count = bucket2To7 },
            new() { Bucket = "8-14 ngày", Count = bucket8To14 },
            new() { Bucket = "15-30 ngày", Count = bucket15To30 },
            new() { Bucket = ">30 ngày", Count = bucketOver30 }
        };

        var result = new LeadAgingResponse
        {
            Items = items,
            StaleLeads = bucket8To14 + bucket15To30 + bucketOver30
        };

        return Results.Ok(ApiResponse.Ok(result, "Get lead aging successfully."));
    }

    private void EnsureStaffOrAdminPermission()
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "1" && role != "2")
        {
            throw new UnauthorizedAccessException("Only admin or staff can view lead analytics.");
        }
    }

    private static void ValidateDateRange(DateOnly? from, DateOnly? to)
    {
        if (from.HasValue && to.HasValue && from.Value > to.Value)
        {
            throw new InvalidOperationException("from must be less than or equal to to.");
        }
    }

    private static string GetSourceName(byte? source)
    {
        return source switch
        {
            1 => "Facebook",
            2 => "Zalo",
            3 => "Referral",
            4 => "Walk-in",
            5 => "Website",
            _ => "Unknown"
        };
    }
}
