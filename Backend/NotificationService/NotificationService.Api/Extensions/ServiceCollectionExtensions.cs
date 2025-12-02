namespace NotificationService.Api.Extensions;

public static class ServiceCollectionExtensions
{
   public static void AddConsumers(this IServiceCollection services)
   {
      services
         .AddTransient<WelcomeUserEventConsumer>()
         .AddTransient<EmailConfirmationEventConsumer>()
         .AddTransient<EmailResetPasswordEventConsumer>();
   }
   
   
   public static void AddServices(this IServiceCollection services)
   {
      services
         .AddTransient<ISmtpEmailService, SmtpEmailService>();
   }
}