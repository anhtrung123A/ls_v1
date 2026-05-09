using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using app.Configurations;

namespace app.Services.Email;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _smtpOptions;
    private readonly IWebHostEnvironment _environment;

    public EmailService(IOptions<SmtpOptions> smtpOptions, IWebHostEnvironment environment)
    {
        _smtpOptions = smtpOptions.Value;
        _environment = environment;
    }

    public async Task SendStaffAccountCreatedAsync(
        string toEmail,
        string fullName,
        string password,
        string staffCode,
        CancellationToken cancellationToken = default)
    {
        EnsureSmtpConfigured();

        var templatePath = Path.Combine(_environment.ContentRootPath, "Views", "Emails", "StaffCreated.html");
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);

        var body = template
            .Replace("{{FullName}}", WebUtility.HtmlEncode(fullName))
            .Replace("{{Email}}", WebUtility.HtmlEncode(toEmail))
            .Replace("{{Password}}", WebUtility.HtmlEncode(password))
            .Replace("{{StaffCode}}", WebUtility.HtmlEncode(staffCode));

        await SendEmailAsync(toEmail, "Your staff account is ready", body, cancellationToken);
    }

    public async Task SendStudentAccountCreatedAsync(
        string toEmail,
        string fullName,
        string password,
        string studentCode,
        CancellationToken cancellationToken = default)
    {
        EnsureSmtpConfigured();

        var templatePath = Path.Combine(_environment.ContentRootPath, "Views", "Emails", "StudentCreated.html");
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);

        var body = template
            .Replace("{{FullName}}", WebUtility.HtmlEncode(fullName))
            .Replace("{{Email}}", WebUtility.HtmlEncode(toEmail))
            .Replace("{{Password}}", WebUtility.HtmlEncode(password))
            .Replace("{{StudentCode}}", WebUtility.HtmlEncode(studentCode));

        await SendEmailAsync(toEmail, "Your student account is ready", body, cancellationToken);
    }

    public async Task SendOtpAsync(
        string toEmail,
        string otp,
        int expireMinutes,
        CancellationToken cancellationToken = default)
    {
        EnsureSmtpConfigured();

        var templatePath = Path.Combine(_environment.ContentRootPath, "Views", "Emails", "OtpVerification.html");
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);

        var body = template
            .Replace("{{Otp}}", WebUtility.HtmlEncode(otp))
            .Replace("{{ExpireMinutes}}", WebUtility.HtmlEncode(expireMinutes.ToString()));

        await SendEmailAsync(toEmail, "Your OTP verification code", body, cancellationToken);
    }

    public async Task SendInvoiceCreatedAsync(
        string toEmail,
        string studentName,
        string className,
        string courseName,
        string roomAddress,
        decimal subtotalAmount,
        decimal discountAmount,
        decimal finalAmount,
        CancellationToken cancellationToken = default)
    {
        EnsureSmtpConfigured();

        var templatePath = Path.Combine(_environment.ContentRootPath, "Views", "Emails", "InvoiceCreated.html");
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);

        var body = template
            .Replace("{{StudentName}}", WebUtility.HtmlEncode(studentName))
            .Replace("{{ClassName}}", WebUtility.HtmlEncode(className))
            .Replace("{{CourseName}}", WebUtility.HtmlEncode(courseName))
            .Replace("{{RoomAddress}}", WebUtility.HtmlEncode(roomAddress))
            .Replace("{{SubtotalAmount}}", WebUtility.HtmlEncode(subtotalAmount.ToString("N2")))
            .Replace("{{DiscountAmount}}", WebUtility.HtmlEncode(discountAmount.ToString("N2")))
            .Replace("{{FinalAmount}}", WebUtility.HtmlEncode(finalAmount.ToString("N2")));

        await SendEmailAsync(toEmail, "New invoice created for your class", body, cancellationToken);
    }

    public async Task SendPaymentConfirmedAsync(
        string toEmail,
        string studentName,
        string className,
        string courseName,
        CancellationToken cancellationToken = default)
    {
        EnsureSmtpConfigured();

        var templatePath = Path.Combine(_environment.ContentRootPath, "Views", "Emails", "PaymentConfirmed.html");
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);

        var body = template
            .Replace("{{StudentName}}", WebUtility.HtmlEncode(studentName))
            .Replace("{{ClassName}}", WebUtility.HtmlEncode(className))
            .Replace("{{CourseName}}", WebUtility.HtmlEncode(courseName));

        await SendEmailAsync(toEmail, "Payment confirmed - class access is now available", body, cancellationToken);
    }

    private void EnsureSmtpConfigured()
    {
        if (string.IsNullOrWhiteSpace(_smtpOptions.Host) ||
            string.IsNullOrWhiteSpace(_smtpOptions.FromEmail))
        {
            throw new InvalidOperationException("SMTP settings are not configured.");
        }
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(_smtpOptions.FromEmail, _smtpOptions.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(_smtpOptions.Host, _smtpOptions.Port);
        await client.SendMailAsync(message, cancellationToken);
    }
}
