namespace Shared.Contracts.Events;

public record EmailResetPasswordEvent(
    string Email,
    string Code);