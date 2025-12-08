namespace UsersService.Services;

public interface IEventBusPublisher
{
    Task PublishWelcomeUserAsync(EmailSignUpEvent @event, CancellationToken cancellationToken);
    Task PublishEmailConfirmationAsync(EmailConfirmationEvent @event, CancellationToken cancellationToken);
    Task PublishEmailResetPasswordAsync(EmailResetPasswordEvent @event, CancellationToken cancellationToken);
    Task PublishEmailChangeAsync(EmailChangeEvent @event, CancellationToken cancellationToken);
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
    
    public async Task PublishWelcomeUserAsync(EmailSignUpEvent @event, CancellationToken cancellationToken) =>
        await PublishAsync(@event, RoutingKeys.Email.SignUp, cancellationToken);

    public async Task PublishEmailConfirmationAsync(EmailConfirmationEvent @event, CancellationToken cancellationToken) =>
        await PublishAsync(@event, RoutingKeys.Email.Confirmation, cancellationToken);

    public async Task PublishEmailResetPasswordAsync(EmailResetPasswordEvent @event, CancellationToken cancellationToken) =>
        await PublishAsync(@event, RoutingKeys.Email.ResetPassword, cancellationToken);

    public async Task PublishEmailChangeAsync(EmailChangeEvent @event, CancellationToken cancellationToken) =>
        await PublishAsync(@event, RoutingKeys.Email.Change, cancellationToken);

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