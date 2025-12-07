namespace Shared.RabbitMq;

public static class RoutingKeys
{
    public const string WelcomeUser = "notification.email.welcome";
    
    public const string EmailConfirmation = "notification.email.confirmation";
    public const string EmailResetPassword = "notification.email.reset-password";
    public const string EmailChange = "notification.email.email-change";

}