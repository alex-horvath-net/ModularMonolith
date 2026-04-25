using System.Security.Claims;

namespace TradingPortal.Security;

public sealed class ClaimsCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser {
    private ClaimsPrincipal User =>
        httpContextAccessor.HttpContext?.User
        ?? new ClaimsPrincipal(new ClaimsIdentity());

    public bool IsAuthenticated =>
        User.Identity?.IsAuthenticated == true;

    public string ExternalUserId =>
        User.FindFirstValue("sub")
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException("Missing external user id claim.");

    public string UserName =>
        User.Identity?.Name
        ?? User.FindFirstValue("preferred_username")
        ?? User.FindFirstValue("name")
        ?? throw new UnauthorizedAccessException("Missing user name claim.");

    public string DisplayName =>
        User.FindFirstValue("name")
        ?? UserName;

    public string? Email =>
        User.FindFirstValue(ClaimTypes.Email)
        ?? User.FindFirstValue("email");

    public string? Desk =>
        User.FindFirstValue("desk");

    public IReadOnlySet<string> Roles =>
        User.FindAll(ClaimTypes.Role)
            .Concat(User.FindAll("roles"))
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.Ordinal);

    public IReadOnlySet<string> Scopes =>
        User.FindAll("scope")
            .Concat(User.FindAll("scp"))
            .SelectMany(x => x.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToHashSet(StringComparer.Ordinal);

    public bool IsInRole(string role) =>
        Roles.Contains(role);

    public bool HasScope(string scope) =>
        Scopes.Contains(scope);
}