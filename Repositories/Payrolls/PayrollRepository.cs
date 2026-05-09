using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Payrolls;

namespace app.Repositories.Payrolls;

public class PayrollRepository : IPayrollRepository
{
    private readonly AppDbContext _db;
    public PayrollRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<PayrollResponse>> GetAllAsync(PayrollListQuery query, CancellationToken cancellationToken = default)
    {
        var payrollQuery =
            from p in _db.Payrolls.AsNoTracking()
            join u in _db.Users.AsNoTracking() on p.UserId equals u.Id into userJoin
            from user in userJoin.DefaultIfEmpty()
            select new PayrollQueryItem { Payroll = p, User = user };

        if (query.UserId.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.UserId == query.UserId.Value);
        if (query.SalaryConfigId.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.SalaryConfigId == query.SalaryConfigId.Value);
        if (query.Month.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.Month == query.Month.Value);
        if (query.Year.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.Year == query.Year.Value);
        if (query.Status.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.Status == query.Status.Value);
        if (query.GeneratedFrom.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.GeneratedAt.HasValue && x.Payroll.GeneratedAt.Value >= query.GeneratedFrom.Value);
        if (query.GeneratedTo.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.GeneratedAt.HasValue && x.Payroll.GeneratedAt.Value <= query.GeneratedTo.Value);
        if (query.CreatedFrom.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.CreatedAt >= query.CreatedFrom.Value);
        if (query.CreatedTo.HasValue) payrollQuery = payrollQuery.Where(x => x.Payroll.CreatedAt <= query.CreatedTo.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim().ToLowerInvariant();
            payrollQuery = payrollQuery.Where(x =>
                (x.User != null && x.User.FullName != null && x.User.FullName.ToLower().Contains(keyword)) ||
                (x.User != null && x.User.Email.ToLower().Contains(keyword)));
        }

        payrollQuery = ApplyOrdering(payrollQuery, query.OrderBy, query.OrderDir);

        var totalItems = await payrollQuery.LongCountAsync(cancellationToken);
        var items = await payrollQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new PayrollResponse
            {
                Id = x.Payroll.Id,
                UserId = x.Payroll.UserId,
                UserFullName = x.User != null ? x.User.FullName : null,
                UserEmail = x.User != null ? x.User.Email : null,
                SalaryConfigId = x.Payroll.SalaryConfigId,
                Month = x.Payroll.Month,
                Year = x.Payroll.Year,
                BaseAmount = x.Payroll.BaseAmount,
                TeachingAmount = x.Payroll.TeachingAmount,
                KpiAmount = x.Payroll.KpiAmount,
                BonusAmount = x.Payroll.BonusAmount,
                DeductionAmount = x.Payroll.DeductionAmount,
                GrossAmount = x.Payroll.GrossAmount,
                NetAmount = x.Payroll.NetAmount,
                Status = x.Payroll.Status,
                GeneratedAt = x.Payroll.GeneratedAt,
                ConfirmedAt = x.Payroll.ConfirmedAt,
                PaidAt = x.Payroll.PaidAt,
                Note = x.Payroll.Note,
                CreatedBy = x.Payroll.CreatedBy,
                CreatedAt = x.Payroll.CreatedAt,
                UpdatedAt = x.Payroll.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResponse<PayrollResponse>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(totalItems / (double)query.PageSize)
        };
    }

    public async Task<PayrollDetailResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var payroll = await (
            from p in _db.Payrolls.AsNoTracking()
            join u in _db.Users.AsNoTracking() on p.UserId equals u.Id into userJoin
            from user in userJoin.DefaultIfEmpty()
            where p.Id == id
            select new PayrollResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                UserFullName = user != null ? user.FullName : null,
                UserEmail = user != null ? user.Email : null,
                SalaryConfigId = p.SalaryConfigId,
                Month = p.Month,
                Year = p.Year,
                BaseAmount = p.BaseAmount,
                TeachingAmount = p.TeachingAmount,
                KpiAmount = p.KpiAmount,
                BonusAmount = p.BonusAmount,
                DeductionAmount = p.DeductionAmount,
                GrossAmount = p.GrossAmount,
                NetAmount = p.NetAmount,
                Status = p.Status,
                GeneratedAt = p.GeneratedAt,
                ConfirmedAt = p.ConfirmedAt,
                PaidAt = p.PaidAt,
                Note = p.Note,
                CreatedBy = p.CreatedBy,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).FirstOrDefaultAsync(cancellationToken);

        if (payroll is null)
        {
            return null;
        }

        var items = await _db.PayrollItems.AsNoTracking()
            .Where(x => x.PayrollId == id)
            .OrderBy(x => x.Id)
            .Select(x => new PayrollItemResponse
            {
                Id = x.Id,
                PayrollId = x.PayrollId,
                Type = x.Type,
                Quantity = x.Quantity,
                UnitAmount = x.UnitAmount,
                Amount = x.Amount,
                ReferenceType = x.ReferenceType,
                ReferenceId = x.ReferenceId,
                Description = x.Description,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PayrollDetailResponse
        {
            Payroll = payroll,
            PayrollItems = items
        };
    }

    public async Task<PayrollResponse?> UpdateStatusAsync(long id, UpdatePayrollStatusRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Status is < 1 or > 4)
        {
            throw new InvalidOperationException("Invalid payroll status.");
        }

        var payroll = await _db.Payrolls.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (payroll is null) return null;

        payroll.Status = request.Status;
        payroll.Note = request.Note;
        payroll.UpdatedAt = DateTime.UtcNow;

        if (request.Status == 2 && !payroll.ConfirmedAt.HasValue)
        {
            payroll.ConfirmedAt = DateTime.UtcNow;
        }
        else if (request.Status == 3 && !payroll.PaidAt.HasValue)
        {
            payroll.PaidAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync(cancellationToken);

        var updated = await (
            from p in _db.Payrolls.AsNoTracking()
            join u in _db.Users.AsNoTracking() on p.UserId equals u.Id into userJoin
            from user in userJoin.DefaultIfEmpty()
            where p.Id == id
            select new PayrollResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                UserFullName = user != null ? user.FullName : null,
                UserEmail = user != null ? user.Email : null,
                SalaryConfigId = p.SalaryConfigId,
                Month = p.Month,
                Year = p.Year,
                BaseAmount = p.BaseAmount,
                TeachingAmount = p.TeachingAmount,
                KpiAmount = p.KpiAmount,
                BonusAmount = p.BonusAmount,
                DeductionAmount = p.DeductionAmount,
                GrossAmount = p.GrossAmount,
                NetAmount = p.NetAmount,
                Status = p.Status,
                GeneratedAt = p.GeneratedAt,
                ConfirmedAt = p.ConfirmedAt,
                PaidAt = p.PaidAt,
                Note = p.Note,
                CreatedBy = p.CreatedBy,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).FirstOrDefaultAsync(cancellationToken);

        return updated;
    }

    private static IQueryable<PayrollQueryItem> ApplyOrdering(IQueryable<PayrollQueryItem> q, string? orderBy, string? orderDir)
    {
        var asc = !string.Equals(orderDir, "desc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();

        return key switch
        {
            "userid" or "user_id" => asc ? q.OrderBy(x => x.Payroll.UserId) : q.OrderByDescending(x => x.Payroll.UserId),
            "year" => asc ? q.OrderBy(x => x.Payroll.Year) : q.OrderByDescending(x => x.Payroll.Year),
            "month" => asc ? q.OrderBy(x => x.Payroll.Month) : q.OrderByDescending(x => x.Payroll.Month),
            "status" => asc ? q.OrderBy(x => x.Payroll.Status) : q.OrderByDescending(x => x.Payroll.Status),
            "generatedat" or "generated_at" => asc ? q.OrderBy(x => x.Payroll.GeneratedAt) : q.OrderByDescending(x => x.Payroll.GeneratedAt),
            "netamount" or "net_amount" => asc ? q.OrderBy(x => x.Payroll.NetAmount) : q.OrderByDescending(x => x.Payroll.NetAmount),
            _ => asc ? q.OrderBy(x => x.Payroll.CreatedAt) : q.OrderByDescending(x => x.Payroll.CreatedAt)
        };
    }

    private sealed class PayrollQueryItem
    {
        public required Payroll Payroll { get; init; }
        public User? User { get; init; }
    }
}
