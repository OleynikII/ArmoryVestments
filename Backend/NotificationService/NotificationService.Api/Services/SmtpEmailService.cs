namespace NotificationService.Api.Services;

public interface ISmtpEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}

public class SmtpEmailService : ISmtpEmailService
{
    private readonly SmtpEmailOptions _smtpEmailOptions;

    public SmtpEmailService(
        IOptions<SmtpEmailOptions> smtpEmailOptions)
    {
        _smtpEmailOptions = smtpEmailOptions.Value;
    }
    
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var fromEmail = _smtpEmailOptions.FromEmail;
        var host = _smtpEmailOptions.Host;
        var port = int.Parse(_smtpEmailOptions.Port);
        var userName = _smtpEmailOptions.UserName;
        var password = _smtpEmailOptions.Password;

        var message = new MailMessage(fromEmail, toEmail, subject, body);
        message.IsBodyHtml = true;
        
        using var client = new SmtpClient(_smtpEmailOptions.Host, port);
        client.Credentials = new NetworkCredential(userName, password);
        client.EnableSsl = true;

        await client.SendMailAsync(message);
    }
}