using System.Collections.Immutable;
using Experts.SecurityOfficer.Common.Infrastructure.Security;
using static Experts.SecurityOfficer.Login.UserStory;
using static Experts.SecurityOfficer.Register.UserStory;

namespace Experts.SecurityOfficer.Register;
internal class Create {
    private UserStory.IAccountStore store;
    private IRandomNumberGenerator random;
    private UserStory.IClock clock;
    private PasswordPolicy passwordPolicy;
    private RoleRolePolicy rolesPolicy;

    internal Create(UserStory.IAccountStore store, IRandomNumberGenerator random, UserStory.IClock clock) {
        this.store = store;
        this.random = random;
        this.clock = clock;
        passwordPolicy = new PasswordPolicy();
        rolesPolicy = new RoleRolePolicy();
    }

    public bool Run(UserStory.Context context) {

        // validate
        if (context.Request is null) {
            context.Response.ErrorMessage = UserStoryConstants.RequestCanNotBeNell;
            return false;
        }

        if (string.IsNullOrWhiteSpace(context.Request.Email)) {
            context.Response.ErrorMessage = UserStoryConstants.EmailIsRequired;
            return false;
        }

        if (!passwordPolicy.IsValid(context.Request.Password)) {
            context.Response.ErrorMessage = UserStoryConstants.PasswordMutBeContain;
            return false;
        }

        if (string.IsNullOrWhiteSpace(context.Request.UserName)) {
            context.Response.ErrorMessage = UserStoryConstants.UserNameIsRequired;
            return false;
        }

        if (!rolesPolicy.IsValid(context.Request.Roles)) {
            context.Response.ErrorMessage = UserStoryConstants.AtLeastOneRoleRequired;
            return false;
        }

        // normalize
        var email = context.Request.Email.Trim().ToLowerInvariant();
        var userName = context.Request.UserName.Trim();
        var roles = context.Request.Roles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();


        var existing = await store.FindByEmailAsync(email, token);
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

        return new UserStoryResponse(account.Id, account.Email, account.UserName, account.Roles);
    }
}

public class PasswordPolicy {
    public bool IsValid(string password) {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (password.Length < 12)
            return false;

        if (!password.Any(char.IsUpper))
            return false;

        if (!password.Any(char.IsLower))
            return false;

        if (!password.Any(char.IsDigit))
            return false;

        if (!password.Any(char.IsLetterOrDigit))
            return false;

        return true;
    }
}

public sealed class RoleRolePolicy {
    private static readonly ImmutableHashSet<string> allowedRoles =
        ["Trader", "RiskManager", "Compliance"];

    public bool IsValid(IEnumerable<string> selectedRoles) {
        if (selectedRoles == null)
            return false;

        if (selectedRoles.Any(role => !allowedRoles.Contains(role)))
            return false;


        return true;
    }
}


