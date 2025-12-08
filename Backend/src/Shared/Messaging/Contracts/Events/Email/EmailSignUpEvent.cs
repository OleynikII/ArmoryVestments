namespace Messaging.Contracts.Events.Email;

public record EmailSignUpEvent(
    string Email,
    string Token,
    string UserName,
    string LastName,
    string FirstName,
    string? MiddleName) : IEvent;