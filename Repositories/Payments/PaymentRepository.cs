using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Payments;

namespace app.Repositories.Payments;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _db;
    public PaymentRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<PaymentResponse>> GetAllAsync(PaymentListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Payments.AsNoTracking().AsQueryable();
        if (query.InvoiceId.HasValue) q = q.Where(x => x.InvoiceId == query.InvoiceId.Value);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var k = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x => (x.TransactionRef != null && x.TransactionRef.ToLower().Contains(k)) || (x.Note != null && x.Note.ToLower().Contains(k)));
        }
        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<PaymentResponse> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalItems = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }

    public async Task<PaymentResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Payments.AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);

    public async Task<PaymentResponse> CreateAsync(CreatePaymentRequest request, long collectedByStaffId, CancellationToken cancellationToken = default)
    {
        if (request.Amount <= 0) throw new InvalidOperationException("amount must be greater than 0.");
        var invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);
        if (invoice is null) throw new InvalidOperationException("Invoice does not exist.");

        var payment = new Payment
        {
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            Method = request.Method,
            Status = 1,
            TransactionRef = request.TransactionRef,
            Note = request.Note,
            CollectedBy = collectedByStaffId,
            PaidAt = request.PaidAt
        };
        _db.Add(payment);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(payment.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created payment.");
    }

    public async Task<PaymentResponse?> ConfirmAsync(long id, CancellationToken cancellationToken = default)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (payment is null) return null;
        if (payment.Status != 1) throw new InvalidOperationException("Only pending payment can be confirmed.");

        var invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == payment.InvoiceId, cancellationToken);
        if (invoice is null) throw new InvalidOperationException("Invoice does not exist.");
        if (payment.Amount < invoice.RemainingAmount)
        {
            throw new InvalidOperationException("Payment amount is less than remaining amount, confirmation rejected.");
        }

        payment.Status = 2;
        payment.PaidAt ??= DateTime.UtcNow;

        invoice.PaidAmount += payment.Amount;
        invoice.RemainingAmount = Math.Max(0, invoice.FinalAmount - invoice.PaidAmount);
        invoice.Status = invoice.RemainingAmount == 0 ? (byte)3 : (byte)2; // paid/partial
        invoice.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    public async Task<PaymentResponse?> RefundAsync(long id, CancellationToken cancellationToken = default)
    {
        var payment = await _db.Payments.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (payment is null) return null;
        if (payment.Status != 2) throw new InvalidOperationException("Only success payment can be refunded.");

        payment.Status = 4;

        var invoice = await _db.Invoices.FirstOrDefaultAsync(x => x.Id == payment.InvoiceId, cancellationToken);
        if (invoice is null) throw new InvalidOperationException("Invoice does not exist.");

        invoice.PaidAmount = Math.Max(0, invoice.PaidAmount - payment.Amount);
        invoice.RemainingAmount = Math.Max(0, invoice.FinalAmount - invoice.PaidAmount);
        invoice.Status = invoice.PaidAmount == 0 ? (byte)1 : (byte)2;
        invoice.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(id, cancellationToken);
    }

    private static IQueryable<Payment> ApplyOrdering(IQueryable<Payment> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "status" => asc ? q.OrderBy(x => x.Status) : q.OrderByDescending(x => x.Status),
            "paid_at" => asc ? q.OrderBy(x => x.PaidAt) : q.OrderByDescending(x => x.PaidAt),
            _ => asc ? q.OrderBy(x => x.CreatedAt) : q.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<Payment, PaymentResponse>> Map() => x => new PaymentResponse
    {
        Id = x.Id,
        InvoiceId = x.InvoiceId,
        Amount = x.Amount,
        Method = x.Method,
        Status = x.Status,
        TransactionRef = x.TransactionRef,
        Note = x.Note,
        CollectedBy = x.CollectedBy,
        PaidAt = x.PaidAt,
        CreatedAt = x.CreatedAt
    };
}
