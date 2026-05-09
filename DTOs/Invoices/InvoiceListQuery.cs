using app.Common.Pagination;

namespace app.DTOs.Invoices;

public class InvoiceListQuery : PaginationQuery
{
    public long? StudentId { get; set; }
    public long? EnrollmentId { get; set; }
    public byte? Status { get; set; }
    public string? Keyword { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDir { get; set; }
}
