namespace Accounts.Domain;

/// <summary>
/// Immutable representation of an authenticated identity that can own roles inside the trading portal.
/// </summary>
public sealed record Account(
    Guid Id,
    string Email,
    string UserName,
    string PasswordHash,
    IReadOnlySet<string> Roles,
    bool IsLocked,
    DateTime CreatedAtUtc);
