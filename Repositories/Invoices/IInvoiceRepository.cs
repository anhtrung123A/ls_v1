using app.Common.Pagination;
using app.DTOs.Invoices;

namespace app.Repositories.Invoices;

public interface IInvoiceRepository
{
    Task<PagedResponse<InvoiceResponse>> GetAllAsync(InvoiceListQuery query, CancellationToken cancellationToken = default);
    Task<InvoiceResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<InvoiceResponse> CreateAsync(CreateInvoiceRequest request, long createdByUserId, CancellationToken cancellationToken = default);
}
