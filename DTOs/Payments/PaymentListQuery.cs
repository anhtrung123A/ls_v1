using app.Common.Pagination;

namespace app.DTOs.Payments;

public class PaymentListQuery : PaginationQuery
{
    public long? InvoiceId { get; set; }
    public byte? Status { get; set; }
    public string? Keyword { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
