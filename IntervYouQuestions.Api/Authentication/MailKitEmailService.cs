
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace IntervYouQuestions.Api.Authentication;

public class MailKitEmailService(IOptions<EmailSettings> emailSettings, ILogger<MailKitEmailService> logger) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;
    private readonly ILogger<MailKitEmailService> _logger = logger;

    public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromAddress));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = htmlMessage };

            using var smtp = new SmtpClient();
            // Use SecureSocketOptions.StartTls for port 587
            // Use SecureSocketOptions.SslOnConnect for port 465
            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

            // Note: only authenticate if your SMTP server requires it
            if (!string.IsNullOrEmpty(_emailSettings.Username) && !string.IsNullOrEmpty(_emailSettings.Password))
            {
                await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
            }

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "Failed to send email to {Email}. Host: {Host}, Port: {Port}",
                toEmail, _emailSettings.SmtpHost, _emailSettings.SmtpPort);
            // Depending on policy, you might re-throw, return false, or just log
            // Consider implementing a retry mechanism or queuing system for resilience
            // For now, we'll let the exception propagate if critical, or just log if not.
            // throw; // Or handle more gracefully
        }
    }
}
