namespace IntervYouQuestions.Api.Authentication;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
}
