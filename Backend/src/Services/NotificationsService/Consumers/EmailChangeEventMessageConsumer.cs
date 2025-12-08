using NotificationsService.Options;
using NotificationsService.Services;

namespace NotificationsService.Consumers;

public class EmailChangeEventMessageConsumer : IMessageConsumer<EmailChangeEvent>
{
    private readonly ClientOptions _clientOptions;
    
    private readonly ISmtpEmailService _smtpEmailService;

    public EmailChangeEventMessageConsumer(
        ISmtpEmailService smtpEmailService,
        IOptions<ClientOptions> clientOptions)
    {
        _smtpEmailService = smtpEmailService;
        _clientOptions = clientOptions.Value;
    }
    
    public async Task ConsumeAsync(EmailChangeEvent @event, CancellationToken cancellationToken)
    {
        var confirmationLink = $@"{_clientOptions.FrontendUrl}/user/confirm-update-email?token={@event.Token}";
        
        var subject = "Email Change CS2 Investment Tracker! 🎮";
        var body = $@"
            <!DOCTYPE html>
            <html>
            <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <div style='background: #667eea; padding: 20px; text-align: center; color: white;'>
                    <h1>Подтверждение Email</h1>
                </div>
                <div style='padding: 30px; background: #f9f9f9;'>
                    <p>Здравствуйте!</p>
                    <p>Для подтверждения нового email перейдите по ссылке:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' 
                           style='background: #667eea; color: white; padding: 12px 24px; 
                                  text-decoration: none; border-radius: 5px;'>
                            Подтвердить
                        </a>
                    </div>
                    <p style='color: #666; font-size: 14px;'>
                        © 2025 Armory Vestments
                    </p>
                </div>
            </body>
            </html>";
         
        await _smtpEmailService.SendEmailAsync(@event.NewEmail, subject, body, cancellationToken);
    }
}