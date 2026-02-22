using System.Collections.Immutable;

namespace Business.Features.Accounts.Slices.Register.WorkSteps;

internal class Validate {
    private readonly CreatePasswordPolicy passwordPolicy = new();
    private readonly CreateRoleRolePolicy rolesPolicy = new();

    public bool Run(UserStory.UserStoryContext context) {

        if (context.Request is null) {
            throw new InvalidOperationException(UserStoryConstants.RequestCanNotBeNell);
        }

        if (string.IsNullOrWhiteSpace(context.Request.Email)) {
            throw new InvalidOperationException(UserStoryConstants.EmailIsRequired);
        }

        if (!passwordPolicy.IsValid(context.Request.Password)) {
            throw new InvalidOperationException(UserStoryConstants.PasswordMutBeContain);
        }

        if (string.IsNullOrWhiteSpace(context.Request.UserName)) {
            throw new InvalidOperationException(UserStoryConstants.UserNameIsRequired);
        }

        if (!rolesPolicy.IsValid(context.Request.Roles)) {
            throw new InvalidOperationException(UserStoryConstants.AtLeastOneRoleRequired);
        }

        return true;
    }
}

internal class CreatePasswordPolicy {
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

internal sealed class CreateRoleRolePolicy {
    private static readonly ImmutableHashSet<string> allowedRoles =
        ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, "Trader", "RiskManager", "Compliance");

    public bool IsValid(IEnumerable<string> selectedRoles) {
        if (selectedRoles == null)
            return false;

        var cleanedRoles = selectedRoles
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Select(role => role.Trim());

        if (!cleanedRoles.Any())
            return false;

        if (cleanedRoles.Any(role => !allowedRoles.Contains(role)))
            return false;

        return true;
    }
}

