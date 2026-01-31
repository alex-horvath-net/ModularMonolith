using System.Collections.ObjectModel;
using Experts.SecurityOfficer.Shared.Domain;
using Experts.SecurityOfficer.Shared.Security;

namespace Experts.SecurityOfficer.Register;

/// <summary>
/// Business command that provisions identities for future authentication.
/// </summary>
public sealed class UserStory {
    private readonly IAccountStore store;
    private readonly IPasswordHasher hasher;
    private readonly IRolePolicy rolePolicy;
    private readonly IClock clock;

    public UserStory(IAccountStore store, IPasswordHasher hasher, IRolePolicy rolePolicy, IClock clock) {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(hasher);
        ArgumentNullException.ThrowIfNull(rolePolicy);
        ArgumentNullException.ThrowIfNull(clock);

        this.store = store;
        this.hasher = hasher;
        this.rolePolicy = rolePolicy;
        this.clock = clock;
    }

    public async Task<Response> RegisterAsync(Request request, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request);

        var email = NormalizeEmail(request.Email);
        var userName = NormalizeUserName(request.UserName);
        var roles = NormalizeRoles(request.Roles);

        if (!PasswordPolicy.IsValid(request.Password)) {
            throw new InvalidOperationException(PasswordPolicy.ValidationMessage);
        }

        if (!rolePolicy.AreEligible(roles)) {
            throw new InvalidOperationException("Requested roles are not eligible for registration.");
        }

        var existing = await store.FindByEmailAsync(email, cancellationToken).ConfigureAwait(false);
        if (existing is not null) {
            throw new InvalidOperationException("An account with this email already exists.");
        }

        var account = new Account(
            Guid.NewGuid(),
            email,
            userName,
            hasher.Hash(request.Password),
            [.. roles],
            IsLocked: false,
            CreatedAtUtc: clock.UtcNow);

        await store.CreateAsync(account, cancellationToken).ConfigureAwait(false);

        return new Response(account.Id, account.Email, account.Roles);
    }

    private static string NormalizeEmail(string email) {
        if (string.IsNullOrWhiteSpace(email)) {
            throw new ArgumentException("Email is required.", nameof(email));
        }

        return email.Trim().ToLowerInvariant();
    }

    private static string NormalizeUserName(string userName) {
        if (string.IsNullOrWhiteSpace(userName)) {
            throw new ArgumentException("User name is required.", nameof(userName));
        }

        return userName.Trim();
    }

    private static IEnumerable<string> NormalizeRoles(IEnumerable<string>? roles) {
        if (roles is null || !roles.Any()) {
            throw new ArgumentException("At least one role must be provided.", nameof(roles));
        }

        var normalized = roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalized.Length == 0) {
            throw new ArgumentException("At least one role must be provided.", nameof(roles));
        }

        return normalized;
    }

    public sealed record Request(
        string Email,
        string UserName,
        string Password,
        IReadOnlyCollection<string> Roles);

    public sealed record Response(Guid AccountId, string Email, IReadOnlyCollection<string> Roles);

    public interface IAccountStore {
        Task<Account?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken);
        Task CreateAsync(Account account, CancellationToken cancellationToken);
    }

    public interface IRolePolicy {
        bool AreEligible(IEnumerable<string> requestedRoles);
    }

    public interface IClock {
        DateTime UtcNow { get; }
    }

    public static class PasswordPolicy {
        public const string ValidationMessage = "Password must be at least 12 characters and contain upper, lower, digit, and symbol.";

        public static bool IsValid(string password) {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 12) {
                return false;
            }

            var hasUpper = password.Any(char.IsUpper);
            var hasLower = password.Any(char.IsLower);
            var hasDigit = password.Any(char.IsDigit);
            var hasSymbol = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSymbol;
        }
    }
}
