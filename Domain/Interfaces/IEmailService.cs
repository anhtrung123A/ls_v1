namespace app.Domain.Interfaces;

public interface IEmailService
{
    Task SendUserCreatedPasswordEmailAsync(
        string toEmail,
        string fullname,
        string generatedPassword,
        CancellationToken cancellationToken = default);
}
