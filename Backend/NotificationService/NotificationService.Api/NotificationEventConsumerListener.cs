namespace NotificationService.Api;

public class NotificationEventConsumerListener : BackgroundService
{
    private readonly IChannel _channel;
    private readonly WelcomeUserEventConsumer _welcomeUserEventConsumer;
    private readonly ILogger<NotificationEventConsumerListener> _logger;
    
    private readonly string _queueName = "notification-service-queue";

    public NotificationEventConsumerListener(
        IConnection connection,
        WelcomeUserEventConsumer welcomeUserEventConsumer,
        ILogger<NotificationEventConsumerListener> logger)
    {
        _welcomeUserEventConsumer = welcomeUserEventConsumer;
        _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;
            
            await ProcessMessageAsync(routingKey, message, ea.BasicProperties, stoppingToken);
            await _channel.BasicAckAsync(
                deliveryTag: ea.DeliveryTag,
                multiple: false,
                cancellationToken: stoppingToken);
        };
        
        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
    }
    
    
    private async Task ProcessMessageAsync(string routingKey, string message, IReadOnlyBasicProperties properties, CancellationToken cancellationToken)
    {
        switch (routingKey)
        {
            case RoutingKeys.WelcomeUser:
                var @event = JsonSerializer.Deserialize<WelcomeUserEvent>(message);
                await _welcomeUserEventConsumer.ConsumeAsync(@event!, cancellationToken);
                break;
            default:
                _logger.LogError("{routingKey} is not found!", routingKey);
                break;
        }
    }
}