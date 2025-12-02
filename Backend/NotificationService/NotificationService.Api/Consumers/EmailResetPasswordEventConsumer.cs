namespace NotificationService.Api.Consumers;

public class EmailResetPasswordEventConsumer : IConsumer<EmailResetPasswordEvent>
{
    private readonly ISmtpEmailService _smtpEmailService;

    public EmailResetPasswordEventConsumer(
        ISmtpEmailService smtpEmailService)
    {
        _smtpEmailService = smtpEmailService;
    }
    
    public async Task ConsumeAsync(
        EmailResetPasswordEvent @event, 
        CancellationToken cancellationToken)
    {
        var subject = "Reset Password Code CS2 Investment Tracker!";
        var body = $@"
            <!DOCTYPE html>
            <html>
            <body style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <div style='background: #dc3545; padding: 20px; color: white; text-align: center;'>
                    <h2>Смена пароля</h2>
                </div>
                <div style='padding: 30px; background: #f8f9fa;'>
                    <p>Здравствуйте!</p>
                    <p>Для смены пароля используйте следующий код подтверждения:</p>
                    
                    <div style='
                        background: white;
                        border: 2px solid #dc3545;
                        border-radius: 10px;
                        padding: 20px;
                        text-align: center;
                        margin: 20px 0;
                        font-size: 32px;
                        font-weight: bold;
                        letter-spacing: 5px;
                        color: #dc3545;
                        font-family: monospace;
                    '>
                        {@event.Code}
                    </div>
                    
                    <p style='color: #666; font-size: 14px;'>
                        Код действителен 10 минут. Никому не сообщайте этот код.
                    </p>
                    
                    <p style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #ddd;'>
                        Если вы не запрашивали смену пароля, проигнорируйте это письмо.
                    </p>
                </div>
            </body>
            </html>";
        await _smtpEmailService.SendEmailAsync(@event.Email, subject, body, cancellationToken);
    }
}