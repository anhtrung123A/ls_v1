namespace app.Domain.Interfaces;

public interface IEmailService
{
    Task SendUserCreatedPasswordEmailAsync(
        string toEmail,
        string fullName,
        string generatedPassword,
        CancellationToken cancellationToken = default);
}
