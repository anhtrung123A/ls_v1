namespace app.Services.Email;

public interface IEmailService
{
    Task SendStaffAccountCreatedAsync(
        string toEmail,
        string fullName,
        string password,
        string staffCode,
        CancellationToken cancellationToken = default);

    Task SendOtpAsync(
        string toEmail,
        string otp,
        int expireMinutes,
        CancellationToken cancellationToken = default);
}
