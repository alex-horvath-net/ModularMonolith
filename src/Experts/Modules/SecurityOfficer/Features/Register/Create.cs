using System.Collections.Immutable;
using Business.Modules.SecurityOfficer.Domain;
using Business.Modules.SecurityOfficer.Infrastructure.Clock;
using Business.Modules.SecurityOfficer.Infrastructure.Hash;

namespace Business.Modules.SecurityOfficer.Features.Register;
internal class Create(ICreateAccountStore store, IHasher hasher, ICreateClock clock) {
    private readonly CreatePasswordPolicy passwordPolicy = new();
    private readonly CreateRoleRolePolicy rolesPolicy = new();

    public async Task<bool> Run(UserStory.UserStoryContext context) {

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
        context.NormalizedRequest = context.Request with {
            Email = context.Request.Email.Trim().ToLowerInvariant(),
            UserName = context.Request.UserName.Trim().ToLowerInvariant(),
            Roles = context.Request.Roles
                        .Where(role => !string.IsNullOrWhiteSpace(role))
                        .Select(role => role.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray()
        };


        context.MathingAccount = await store.FindAccountByEmail(context.NormalizedRequest.Email, context.Token);
        if (context.MathingAccount is not null) {
            context.Response.ErrorMessage = UserStoryConstants.AccountAlreadyExists;
            return false;
        }

        context.Account = new Account(
            Id: Guid.NewGuid(),
            Email: context.NormalizedRequest.Email,
            UserName: context.NormalizedRequest.UserName,
            PasswordHash: hasher.Generate(context.NormalizedRequest.Password),
            Roles: context.Request.Roles.ToHashSet(),
            IsLocked: false,
            CreatedAtUtc: clock.UtcNow);

        await store.CreateAccount(context.Account, context.Token);

        context.Response.AccountId = context.Account.Id;
        context.Response.Email = context.Account.Email;
        context.Response.UserName = context.Account.UserName;
        context.Response.Roles = context.Account.Roles;

        return true;
    }
}

public interface ICreateAccountStore {
    Task<Account?> FindAccountByEmail(string email, CancellationToken token);
    Task<Account?> CreateAccount(Account account, CancellationToken token);
}

public interface ICreateClock : IClock { }

public sealed class CreateClock : SystemClock, ICreateClock { }

public class CreatePasswordPolicy {
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

public sealed class CreateRoleRolePolicy {
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
