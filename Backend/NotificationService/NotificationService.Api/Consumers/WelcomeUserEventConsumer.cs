namespace NotificationService.Api.Consumers;

public class WelcomeUserEventConsumer : IConsumer<WelcomeUserEvent>
{
    private readonly ISmtpEmailService _smtpEmailService;
    
    public WelcomeUserEventConsumer(
        ISmtpEmailService smtpEmailService)
    {
        _smtpEmailService = smtpEmailService;
    }

    public async Task ConsumeAsync(WelcomeUserEvent @event, CancellationToken cancellationToken)
    {
        var subject = "Welcome to CS2 Investment Tracker! 🎮";
        var body = $"""
                    <div style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                        <h2 style="color: #333;">Welcome, {@event.LastName} {@event.FirstName} {@event.MiddleName}! 👋</h2>
                        <p>Thank you for joining <strong>CS2 Investment Tracker</strong> - your ultimate tool for tracking CS2 skin investments!</p>
                        
                        <div style="background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0;">
                            <h3 style="color: #666; margin-top: 0;">Get Started:</h3>
                            <ul>
                                <li>📊 Track your skin portfolio performance</li>
                                <li>📈 Monitor market price fluctuations</li>
                                <li>💰 Calculate your profit & loss</li>
                                <li>🔔 Set up price alerts</li>
                            </ul>
                        </div>
                        
                        <p><strong>Username:</strong> {@event.UserName}</p>
                        
                        <hr style="border: none; border-top: 1px solid #eee; margin: 30px 0;">
                        <p style="color: #666; font-size: 12px;">
                            Happy investing!<br>
                            The CS2 Investment Tracker Team
                        </p>
                    </div>
                    """;
        
        await _smtpEmailService.SendEmailAsync(@event.Email, subject, body, cancellationToken);
    }
}