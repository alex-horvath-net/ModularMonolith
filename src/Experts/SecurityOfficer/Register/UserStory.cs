using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Infrastructure.Security;
using Experts.SecurityOfficer.Register.Infrastructure;

namespace Experts.SecurityOfficer.Register;

/// <summary>
/// Business command that provisions identities for future authentication.
/// </summary>
internal sealed class UserStory {
    private readonly IAccountStore store;
    private readonly Pbkdf2PasswordHasher hasher;
    private readonly DefaultRolePolicy rolePolicy;
    private readonly IClock clock;

    public UserStory(
        IAccountStore store,
        IRandomNumberGenerator random,
        IClock clock) {
        ArgumentNullException.ThrowIfNull(store);
        ArgumentNullException.ThrowIfNull(random);
        ArgumentNullException.ThrowIfNull(rolePolicy);
        ArgumentNullException.ThrowIfNull(clock);

        this.store = store;
        hasher = new Pbkdf2PasswordHasher(random);
        rolePolicy = new DefaultRolePolicy();
        this.clock = clock;
    }

    public async Task<Response> Register(Request request, CancellationToken token) {
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

        var existing = await store.FindByEmailAsync(email, token).ConfigureAwait(false);
        if (existing is not null) {
            throw new InvalidOperationException("An account with this email already exists.");
        }

        var account = new Account(
            Guid.NewGuid(),
            email,
            userName,
            hasher.Hash(request.Password),
            new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase),
            IsLocked: false,
            CreatedAtUtc: clock.UtcNow);

        await store.CreateAsync(account, token).ConfigureAwait(false);

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
        Task<Account?> FindByEmailAsync(string email, CancellationToken token);
        Task CreateAsync(Account account, CancellationToken token);
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
