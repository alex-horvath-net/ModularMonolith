using System.Collections.Immutable;

namespace Business.Experts.SecurityOfficer.UserStories.Register.WorkSteps;
internal class Validate {
    private readonly CreatePasswordPolicy passwordPolicy = new();
    private readonly CreateRoleRolePolicy rolesPolicy = new();
    public bool Run(UserStory.UserStoryContext context) {

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
        ["Trader", "RiskManager", "Compliance"];

    public bool IsValid(IEnumerable<string> selectedRoles) {
        if (selectedRoles == null)
            return false;

        if (selectedRoles.Any(role => !allowedRoles.Contains(role)))
            return false;


        return true;
    }
}

