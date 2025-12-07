var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Options
var jwtOptions = configuration
    .GetRequiredSection(nameof(JwtOptions))
    .Get<JwtOptions>();

if (jwtOptions == null) throw new ArgumentNullException("JwtOptions is missing!");

builder.Services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
// Options

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureSwagger();

builder.Services.AddCors(options => options.AddDefaultPolicy(
    policy => policy.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()));

builder.AddNpgsqlDbContext<UserServiceDbContext>("userservicedb");
builder.Services.ConfigureJwtBearer(jwtOptions);
builder.Services.AddEndpoints();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddHttpContextAccessor();

builder.Services.AddHelpers();
builder.Services.AddRepositories();

builder.AddRabbitMQClient(connectionName: "rabbitmq");

builder.Services.AddScoped<IEventBusPublisher, EventBusPublisher>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.UseMiddleware<ErrorHandlerMiddleware>();

await using var serviceScope = app.Services.CreateAsyncScope();
await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<UserServiceDbContext>();
await dbContext.Database.EnsureCreatedAsync();

app.Run();