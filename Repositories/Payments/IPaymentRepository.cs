using app.Common.Pagination;
using app.DTOs.Payments;

namespace app.Repositories.Payments;

public interface IPaymentRepository
{
    Task<PagedResponse<PaymentResponse>> GetAllAsync(PaymentListQuery query, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<PaymentResponse> CreateAsync(CreatePaymentRequest request, long collectedByStaffId, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> ConfirmAsync(long id, CancellationToken cancellationToken = default);
    Task<PaymentResponse?> RefundAsync(long id, CancellationToken cancellationToken = default);
}
