namespace Messaging.Contracts.Events.Email;

public record EmailConfirmationEvent(
    string Email,
    string Token) : IEvent;