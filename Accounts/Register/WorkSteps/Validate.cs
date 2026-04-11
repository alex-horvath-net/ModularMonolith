using System.Collections.Immutable;
using Accounts.Register.UserStory;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register.WorkSteps;

internal sealed class Validate(
    IClock clock,
    IGuid guid,
    ILogger<Validate> logger) : WorkStep<Context>(clock, guid, logger) {
    protected override Task Run(Context context) {
        context.ExecutedBusinessWorkSteps.Add(RegistrationWorkStep.Validation);

        if (context.Request is null) {
            throw new InvalidOperationException(Constants.RequestCanNotBeNull);
        }

        if (string.IsNullOrWhiteSpace(context.Request.Email)) {
            throw new InvalidOperationException(Constants.EmailIsRequired);
        }

        if (!passwordPolicy.IsValid(context.Request.Password)) {
            throw new InvalidOperationException(Constants.PasswordMustBeContain);
        }

        if (string.IsNullOrWhiteSpace(context.Request.UserName)) {
            throw new InvalidOperationException(Constants.UserNameIsRequired);
        }

        if (!rolesPolicy.IsValid(context.Request.Roles)) {
            throw new InvalidOperationException(Constants.AtLeastOneRoleRequired);
        }

        return Task.CompletedTask;
    }

    private readonly CreatePasswordPolicy passwordPolicy = new();
    private readonly CreateRoleRolePolicy rolesPolicy = new();

    private sealed class CreatePasswordPolicy {
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

            if (!password.Any(character => !char.IsLetterOrDigit(character)))
                return false;

            return true;
        }
    }

    private sealed class CreateRoleRolePolicy {
        private static readonly ImmutableHashSet<string> allowedRoles =
            ImmutableHashSet.Create(StringComparer.OrdinalIgnoreCase, "Trader", "RiskManager", "Compliance");

        public bool IsValid(IEnumerable<string> selectedRoles) {
            if (selectedRoles == null)
                return false;

            if (selectedRoles.Any(role => !allowedRoles.Contains(role)))
                return false;

            return true;
        }
    }
}

