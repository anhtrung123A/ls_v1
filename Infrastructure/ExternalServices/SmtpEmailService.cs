using System.Net;
using System.Net.Mail;
using app.Domain.Interfaces;
using app.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace app.Infrastructure.ExternalServices;

public class SmtpEmailService : IEmailService
{
    private const string TemplatePath = "MailTemplates/UserCreatedPasswordTemplate.html";
    private readonly SmtpOptions _options;

    public SmtpEmailService(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendUserCreatedPasswordEmailAsync(
        string toEmail,
        string fullName,
        string generatedPassword,
        CancellationToken cancellationToken = default)
    {
        var template = await LoadTemplateAsync(cancellationToken);
        var htmlBody = template
            .Replace("{{FullName}}", WebUtility.HtmlEncode(fullName))
            .Replace("{{Email}}", WebUtility.HtmlEncode(toEmail))
            .Replace("{{GeneratedPassword}}", WebUtility.HtmlEncode(generatedPassword));

        using var smtpClient = new SmtpClient(_options.Host, _options.Port);
        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = "Your account has been created",
            Body = htmlBody,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);
        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }

    private static async Task<string> LoadTemplateAsync(CancellationToken cancellationToken)
    {
        var templateFilePath = Path.Combine(AppContext.BaseDirectory, TemplatePath);
        return await File.ReadAllTextAsync(templateFilePath, cancellationToken);
    }
}
