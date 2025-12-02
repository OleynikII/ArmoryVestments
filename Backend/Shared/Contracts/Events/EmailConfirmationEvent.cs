namespace Shared.Contracts.Events;

public record EmailConfirmationEvent(
    string Email,
    string Token);