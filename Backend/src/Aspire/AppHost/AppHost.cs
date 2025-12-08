var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.AddRabbitMQ("rabbitmq",
        port: 5672)
    .WithManagementPlugin(port: 59744)
    .WithDataVolume();

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithDataVolume();

var userServiceDb = postgres.AddDatabase("userservicedb");

var userService = builder.AddProject<UsersService>("user-service")
    .WithReference(userServiceDb)
    .WithReference(rabbitMq)
    .WaitFor(userServiceDb)
    .WithEnvironment("JwtOptions__SigningKey", "ArmoryVestments-at-least-32-chars-long-nasral-key")
    .WithEnvironment("JwtOptions__Issuer", "cs2-armory-vestments")
    .WithEnvironment("JwtOptions__Audience", "cs2-armory-vestments")
    .WithEnvironment("JwtOptions__ExpirationSeconds", "900");



var notificationService = builder.AddProject<NotificationsService>("notification-service")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq)
    .WithEnvironment("SmtpEmailOptions__Host", "smtp.gmail.com")
    .WithEnvironment("SmtpEmailOptions__Port", "587")
    .WithEnvironment("SmtpEmailOptions__UserName", "armoryvestments@gmail.com")
    .WithEnvironment("SmtpEmailOptions__Password", "yisx vwmz szmb qbgd")
    .WithEnvironment("SmtpEmailOptions__FromEmail", "armoryvestments@gmail.com")
    .WithEnvironment("ClientOptions__FrontendUrl", "https://example-domain.ru");

builder.Build().Run();