namespace Shared.Contracts.Events;

public record WelcomeUserEvent(
    string Email,
    string Token,
    string UserName,
    string LastName,
    string FirstName,
    string? MiddleName
);