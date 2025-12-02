namespace UserService.Api.Services;

public interface IEventBusPublisher
{
    Task PublishWelcomeUserAsync(WelcomeUserEvent @event, CancellationToken cancellationToken);
    Task PublishEmailConfirmationAsync(EmailConfirmationEvent @event, CancellationToken cancellationToken);
    Task PublishEmailResetPasswordAsync(EmailResetPasswordEvent @event, CancellationToken cancellationToken);
}

public class EventBusPublisher : IEventBusPublisher
{
    private readonly IChannel _channel;
    private readonly string _exchangeName = "notifications-topic";

    public EventBusPublisher(
        IConnection connection)
    {
        _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
    }
    
    public async Task PublishWelcomeUserAsync(WelcomeUserEvent @event, CancellationToken cancellationToken) =>
        await PublishAsync(@event, RoutingKeys.WelcomeUser, cancellationToken);

    public async Task PublishEmailConfirmationAsync(EmailConfirmationEvent @event, CancellationToken cancellationToken) =>
        await PublishAsync(@event, RoutingKeys.EmailVerification, cancellationToken);

    public async Task PublishEmailResetPasswordAsync(EmailResetPasswordEvent @event, CancellationToken cancellationToken) =>
        await PublishAsync(@event, RoutingKeys.EmailResetPassword, cancellationToken);

    private async Task PublishAsync<T>(T @event, string routingKey, CancellationToken cancellationToken) where T : class
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        
        await _channel.BasicPublishAsync(
            exchange: _exchangeName,
            routingKey: routingKey,
            mandatory: true,
            body: body,
            cancellationToken: cancellationToken);
    }
}