using NotificationsService.Consumers;
using NotificationsService.Services;

namespace NotificationsService.Extensions;

public static class ServiceCollectionExtensions
{
   public static void AddConsumers(this IServiceCollection services)
   {
      services
         .AddTransient<EmailSignUpEventMessageConsumer>()
         .AddTransient<EmailConfirmationEventMessageConsumer>()
         .AddTransient<EmailResetPasswordEventMessageConsumer>()
         .AddTransient<EmailChangeEventMessageConsumer>();
   }
   
   
   public static void AddServices(this IServiceCollection services)
   {
      services
         .AddTransient<ISmtpEmailService, SmtpEmailService>();
   }
}