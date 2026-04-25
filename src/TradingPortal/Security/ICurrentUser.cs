namespace TradingPortal.Security;

public interface ICurrentUser {
    bool IsAuthenticated { get; }

    string ExternalUserId { get; }

    string UserName { get; }

    string DisplayName { get; }

    string? Email { get; }

    string? Desk { get; }

    IReadOnlySet<string> Roles { get; }

    IReadOnlySet<string> Scopes { get; }

    bool IsInRole(string role);

    bool HasScope(string scope);
}