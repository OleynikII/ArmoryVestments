namespace Shared.RabbitMq;

public static class RoutingKeys
{
    public const string WelcomeUser = "notification.email.welcome";
    
    public const string EmailVerification = "notification.email.verification";
    public const string EmailVerified = "notification.email.verified";
    
    public const string PasswordReset = "notification.security.password-reset";
}