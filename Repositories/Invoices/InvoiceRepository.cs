using Microsoft.EntityFrameworkCore;
using app.Common.Pagination;
using app.Data.EF;
using app.Data.EF.Entities;
using app.DTOs.Invoices;

namespace app.Repositories.Invoices;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly AppDbContext _db;
    public InvoiceRepository(AppDbContext db) { _db = db; }

    public async Task<PagedResponse<InvoiceResponse>> GetAllAsync(InvoiceListQuery query, CancellationToken cancellationToken = default)
    {
        var q = _db.Invoices.AsNoTracking().AsQueryable();
        if (query.StudentId.HasValue) q = q.Where(x => x.StudentId == query.StudentId.Value);
        if (query.EnrollmentId.HasValue) q = q.Where(x => x.EnrollmentId == query.EnrollmentId.Value);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var k = query.Keyword.Trim().ToLowerInvariant();
            q = q.Where(x => x.InvoiceNo.ToLower().Contains(k));
        }

        q = ApplyOrdering(q, query.OrderBy, query.OrderDir);
        var total = await q.LongCountAsync(cancellationToken);
        var items = await q.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(Map()).ToListAsync(cancellationToken);
        return new PagedResponse<InvoiceResponse> { Items = items, Page = query.Page, PageSize = query.PageSize, TotalItems = total, TotalPages = (int)Math.Ceiling(total / (double)query.PageSize) };
    }

    public async Task<InvoiceResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _db.Invoices.AsNoTracking().Where(x => x.Id == id).Select(Map()).FirstOrDefaultAsync(cancellationToken);

    public async Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, long createdByUserId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _db.Enrollments.FirstOrDefaultAsync(x => x.Id == request.EnrollmentId, cancellationToken);
        if (enrollment is null) throw new InvalidOperationException("Enrollment does not exist.");
        var studentExists = await _db.Students.AnyAsync(x => x.Id == request.StudentId, cancellationToken);
        if (!studentExists) throw new InvalidOperationException("Student does not exist.");
        if (enrollment.StudentId != request.StudentId) throw new InvalidOperationException("Student does not match enrollment.");

        if (!enrollment.TuitionFee.HasValue) throw new InvalidOperationException("Enrollment tuition_fee is empty.");
        var subtotal = enrollment.TuitionFee.Value;
        var discount = enrollment.Discount;
        var finalAmount = enrollment.FinalFee ?? (subtotal - discount);
        var invoice = new Invoice
        {
            InvoiceNo = await GenerateInvoiceNoAsync(cancellationToken),
            EnrollmentId = request.EnrollmentId,
            StudentId = request.StudentId,
            SubtotalAmount = subtotal,
            DiscountAmount = discount,
            FinalAmount = finalAmount,
            PaidAmount = 0,
            RemainingAmount = finalAmount,
            DueDate = request.DueDate,
            Status = 1,
            Note = request.Note,
            CreatedBy = createdByUserId,
            UpdatedAt = DateTime.UtcNow
        };
        _db.Add(invoice);
        await _db.SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(invoice.Id, cancellationToken) ?? throw new InvalidOperationException("Cannot read newly created invoice.");
    }

    private async Task<string> GenerateInvoiceNoAsync(CancellationToken cancellationToken)
    {
        var prefix = $"INV{DateTime.UtcNow:yyyyMM}";
        var count = await _db.Invoices.LongCountAsync(x => x.InvoiceNo.StartsWith(prefix), cancellationToken);
        return $"{prefix}{(count + 1):D4}";
    }

    private static IQueryable<Invoice> ApplyOrdering(IQueryable<Invoice> q, string? orderBy, string? orderDir)
    {
        var asc = string.Equals(orderDir, "asc", StringComparison.OrdinalIgnoreCase);
        var key = orderBy?.Trim().ToLowerInvariant();
        return key switch
        {
            "invoice_no" => asc ? q.OrderBy(x => x.InvoiceNo) : q.OrderByDescending(x => x.InvoiceNo),
            "status" => asc ? q.OrderBy(x => x.Status) : q.OrderByDescending(x => x.Status),
            "due_date" => asc ? q.OrderBy(x => x.DueDate) : q.OrderByDescending(x => x.DueDate),
            _ => asc ? q.OrderBy(x => x.CreatedAt) : q.OrderByDescending(x => x.CreatedAt)
        };
    }

    private static System.Linq.Expressions.Expression<Func<Invoice, InvoiceResponse>> Map() => x => new InvoiceResponse
    {
        Id = x.Id,
        InvoiceNo = x.InvoiceNo,
        EnrollmentId = x.EnrollmentId,
        StudentId = x.StudentId,
        SubtotalAmount = x.SubtotalAmount,
        DiscountAmount = x.DiscountAmount,
        FinalAmount = x.FinalAmount,
        PaidAmount = x.PaidAmount,
        RemainingAmount = x.RemainingAmount,
        DueDate = x.DueDate,
        Status = x.Status,
        Note = x.Note,
        CreatedBy = x.CreatedBy,
        CreatedAt = x.CreatedAt,
        UpdatedAt = x.UpdatedAt
    };
}
