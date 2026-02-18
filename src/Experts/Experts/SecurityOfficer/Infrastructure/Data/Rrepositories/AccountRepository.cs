using Business.Experts.SecurityOfficer.Domain;
using Common.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Business.Experts.SecurityOfficer.Infrastructure.Data.Rrepositories;

public sealed class AccountRepository(SecurityDbContext db) : IAccountRepository {
    public async Task<Account?> FindAccountByEmail(string email, CancellationToken token) => await db.Accounts
        .AsNoTracking()
        .FirstOrDefaultAsync(account => account.EmailNormalized == email, token)
        .Map(ToDomain);

    public async Task CreateAccount(Account account, CancellationToken token) => await account
        .Map(ToData)
        .Then(data => db.Accounts.Add(data))
        .Then(() => db.SaveChangesAsync(token));

    private static Models.Account ToData(Account domain) => new() {
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

    private static Models.Role MapToData(string role) => new() {
        Name = role,
        NormalizedName = role
    };

    private static Account? ToDomain(Models.Account? account) =>
        account is null ? null : new(
            account.Id,
            account.EmailNormalized,
            account.UserNameNormalized,
            account.PasswordHash,
            account.Roles.Select(MapToDomain).ToHashSet(),
            account.IsLocked,
            account.CreatedAtUtc);

    private static string MapToDomain(Models.Role role) =>
        role.Name;
}


