using System.Collections.Immutable;
using Business.Experts.SecurityOfficer.Domain;
using Business.Experts.SecurityOfficer.Infrastructure.Clock;
using Business.Experts.SecurityOfficer.Infrastructure.Data;
using Business.Experts.SecurityOfficer.Infrastructure.Data.Models;
using Business.Experts.SecurityOfficer.Infrastructure.Hash;

namespace Business.Experts.SecurityOfficer.UserStories.Register;
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

public sealed class AccountStore(SecurityOfficerDbContext db) : ICreateAccountStore {
    public async Task<Account?> FindAccountByEmail(string email, CancellationToken token) {

        var entity = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(account => account.EmailNormalized == email, token)
            .ConfigureAwait(false);

        return MapToDomain(entity);
    }

    public async Task CreateAccount(Account account, CancellationToken token) {

        var data = MapToData(account)!;

        db.Accounts.Add(data);

        await db
            .SaveChangesAsync(token);
    }

    private static SecurityOfficer.Infrastructure.Data.Models.Account MapToData(Account domain) => new() {
        Id = domain.Id,
        Email = domain.Email,
        EmailNormalized = domain.Email,
        UserName = domain.UserName,
        UserNameNormalized = domain.UserName,
        PasswordHash = domain.PasswordHash,
        PasswordChangedAtUtc = domain.CreatedAtUtc,
        Roles = domain.Roles.Select(MapToData).ToHashSet(),
        IsLocked = domain.IsLocked,
        FailedAccessCount = 0,
        CreatedAtUtc = domain.CreatedAtUtc,
        UpdatedAtUtc = domain.CreatedAtUtc,
        IsDeleted = false
    };

    private static Role MapToData(string role) => new() {
        Name = role,
        NormalizedName = role
    };

    private static Account? MapToDomain(SecurityOfficer.Infrastructure.Data.Models.Account? entity) =>
        entity is null ? null : new(
            entity.Id,
            entity.Email,
            entity.UserName,
            entity.PasswordHash,
            entity.Roles.Select(MapToDomain).ToHashSet(),
            entity.IsLocked,
            entity.CreatedAtUtc);

    private static string MapToDomain(Role data) =>
        data.Name;
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
