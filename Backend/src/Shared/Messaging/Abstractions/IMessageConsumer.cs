namespace Messaging.Abstractions;

public interface IMessageConsumer<in TEvent> where TEvent : IEvent
{
    Task ConsumeAsync(TEvent @event, CancellationToken cancellationToken);
};