namespace NotificationService.Api.Extensions;

public static class ServiceCollectionExtensions
{
   
   public static void AddServices(this IServiceCollection services)
   {
      services
         .AddTransient<ISmtpEmailService, SmtpEmailService>();
   }
}