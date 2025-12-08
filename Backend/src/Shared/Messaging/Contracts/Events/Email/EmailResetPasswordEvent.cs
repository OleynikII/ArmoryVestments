namespace Messaging.Contracts.Events.Email;

public record EmailResetPasswordEvent(
    string Email,
    string Code) : IEvent;