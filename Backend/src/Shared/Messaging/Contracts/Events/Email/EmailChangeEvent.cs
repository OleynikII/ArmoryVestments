namespace Messaging.Contracts.Events.Email;

public record EmailChangeEvent(
    string NewEmail,
    string Token) : IEvent;