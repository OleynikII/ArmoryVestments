namespace UserService.Api.Services;

public interface IEventBusPublisher
{
    Task PublishWelcomeUserAsync(WelcomeUserEvent @event, CancellationToken cancellationToken);
}

public class EventBusPublisher : IEventBusPublisher
{
    private readonly IConnection _connection;
    private readonly string _exchangeName = "notifications-topic";


    public EventBusPublisher(
        IConnection connection)
    {
        _connection = connection;
    }
    
    public Task PublishWelcomeUserAsync(WelcomeUserEvent @event, CancellationToken cancellationToken) =>
        PublishAsync(@event, RoutingKeys.WelcomeUser, cancellationToken);

    private async Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken) where T : class
    {
        await using var channel = await _connection.CreateChannelAsync(
            cancellationToken: cancellationToken);
        
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        
        await channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: true,
            body: body,
            cancellationToken: cancellationToken);
    }
}