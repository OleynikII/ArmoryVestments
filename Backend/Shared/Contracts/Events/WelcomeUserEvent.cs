namespace Shared.Contracts.Events;

public record WelcomeUserEvent(
    string Email,
    string UserName,
    string LastName,
    string FirstName,
    string? MiddleName
);