var builder = DistributedApplication.CreateBuilder(args);

var rabbitMq = builder.AddRabbitMQ("rabbitmq",
        port: 5672)
    .WithManagementPlugin(port: 59744)
    .WithLifetime(ContainerLifetime.Persistent);

var postgres = builder.AddPostgres("postgres")
    .WithPgAdmin()
    .WithLifetime(ContainerLifetime.Persistent);

var userServiceDb = builder.AddPostgres("userservicedb")
    .WithLifetime(ContainerLifetime.Persistent);

var userService = builder.AddProject<UserService_Api>("user-service")
    .WithReference(userServiceDb)
    .WithReference(rabbitMq)
    .WaitFor(userServiceDb);


var notificationService = builder.AddProject<NotificationService_Api>("notification-service")
    .WithReference(rabbitMq)
    .WaitFor(rabbitMq);

builder.Build().Run();