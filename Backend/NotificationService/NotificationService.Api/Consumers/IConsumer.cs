using Shared.Contracts.Events;

namespace NotificationService.Api.Consumers;

public interface IConsumer<in TEvent>
{
    Task ConsumeAsync(TEvent @event, CancellationToken cancellationToken);
}