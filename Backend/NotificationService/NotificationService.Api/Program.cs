var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Options
builder.Services.Configure<SmtpEmailOptions>(configuration.GetSection(nameof(SmtpEmailOptions)));

builder.AddServiceDefaults();

builder.Services.AddServices();

builder.AddRabbitMQClient(connectionName: "rabbitmq");

builder.Services.AddTransient<WelcomeUserEventConsumer>();
builder.Services.AddHostedService<NotificationEventConsumerListener>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();
