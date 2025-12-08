using Messaging.Constants;
using Messaging.Contracts.Events.Email;
using NotificationsService.Consumers;

namespace NotificationsService;

public class NotificationEventConsumerListener : BackgroundService
{
    private readonly IChannel _channel;
    private readonly ILogger<NotificationEventConsumerListener> _logger;
    
    private readonly EmailSignUpEventMessageConsumer _emailSignUpEventMessageConsumer;
    private readonly EmailConfirmationEventMessageConsumer _emailConfirmationEventMessageConsumer;
    private readonly EmailResetPasswordEventMessageConsumer _emailResetPasswordEventMessageConsumer;
    private readonly EmailChangeEventMessageConsumer _emailChangeEventMessageConsumer;
    
    private readonly string _queueName = "notification-service-queue";

    public NotificationEventConsumerListener(
        IConnection connection,
        ILogger<NotificationEventConsumerListener> logger,
        EmailSignUpEventMessageConsumer emailSignUpEventMessageConsumer,
        EmailConfirmationEventMessageConsumer emailConfirmationEventMessageConsumer,
        EmailResetPasswordEventMessageConsumer emailResetPasswordEventMessageConsumer,
        EmailChangeEventMessageConsumer emailChangeEventMessageConsumer)
    {
        _channel = connection.CreateChannelAsync().GetAwaiter().GetResult();
        _logger = logger;
        
        _emailSignUpEventMessageConsumer = emailSignUpEventMessageConsumer;
        _emailConfirmationEventMessageConsumer = emailConfirmationEventMessageConsumer;
        _emailResetPasswordEventMessageConsumer = emailResetPasswordEventMessageConsumer;
        _emailChangeEventMessageConsumer = emailChangeEventMessageConsumer;
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
            case RoutingKeys.Email.SignUp:
                var emailSignUpEvent = JsonSerializer.Deserialize<EmailSignUpEvent>(message);
                await _emailSignUpEventMessageConsumer.ConsumeAsync(emailSignUpEvent!, cancellationToken);
                break;
            
            case RoutingKeys.Email.Confirmation:
                var emailConfirmationEvent = JsonSerializer.Deserialize<EmailConfirmationEvent>(message);
                await _emailConfirmationEventMessageConsumer.ConsumeAsync(emailConfirmationEvent!, cancellationToken);
                break;
            
            case RoutingKeys.Email.ResetPassword:
                var emailResetPasswordEvent = JsonSerializer.Deserialize<EmailResetPasswordEvent>(message);
                await _emailResetPasswordEventMessageConsumer.ConsumeAsync(emailResetPasswordEvent!, cancellationToken);
                break;
            
            case RoutingKeys.Email.Change:
                var emailChangeEvent = JsonSerializer.Deserialize<EmailChangeEvent>(message);
                await _emailChangeEventMessageConsumer.ConsumeAsync(emailChangeEvent!, cancellationToken);
                break;
            
            default:
                _logger.LogError("{routingKey} is not found!", routingKey);
                break;
        }
    }
}