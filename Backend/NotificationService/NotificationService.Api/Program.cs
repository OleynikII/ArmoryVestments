var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Options
var smtpEmailOptions = configuration
    .GetRequiredSection(nameof(SmtpEmailOptions))
    .Get<SmtpEmailOptions>(binderOptions => binderOptions.BindNonPublicProperties = true);

if (smtpEmailOptions == null) throw new ArgumentNullException("SmtpEmailOptions is missing!");

builder.Services.Configure<SmtpEmailOptions>(configuration.GetSection(nameof(SmtpEmailOptions)));

builder.Services.Configure<ClientOptions>(configuration.GetSection(nameof(ClientOptions)));
// Options

builder.AddServiceDefaults();

builder.Services.AddServices();

builder.AddRabbitMQClient(connectionName: "rabbitmq");

builder.Services.AddConsumers();
builder.Services.AddHostedService<NotificationEventConsumerListener>();

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.Run();
