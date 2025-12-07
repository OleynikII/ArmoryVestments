namespace Shared.Contracts.Events;

public record EmailChangeEvent(
    string NewEmail,
    string Token);