namespace app.Services.Email;

public interface IEmailService
{
    Task SendStaffAccountCreatedAsync(
        string toEmail,
        string fullName,
        string password,
        string staffCode,
        CancellationToken cancellationToken = default);

    Task SendStudentAccountCreatedAsync(
        string toEmail,
        string fullName,
        string password,
        string studentCode,
        CancellationToken cancellationToken = default);

    Task SendOtpAsync(
        string toEmail,
        string otp,
        int expireMinutes,
        CancellationToken cancellationToken = default);

    Task SendInvoiceCreatedAsync(
        string toEmail,
        string studentName,
        string className,
        string courseName,
        string roomAddress,
        decimal subtotalAmount,
        decimal discountAmount,
        decimal finalAmount,
        CancellationToken cancellationToken = default);

    Task SendPaymentConfirmedAsync(
        string toEmail,
        string studentName,
        string className,
        string courseName,
        CancellationToken cancellationToken = default);
}
