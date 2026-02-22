using Common.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Business.Features.Accounts.Infrastructure.Data.Rrepositories;

public sealed class AccountRepository(SecurityDbContext db) : IAccountRepository {
    public async Task<Domain.Account?> FindAccountByEmail(string email, CancellationToken token) => await db.Accounts
        .AsNoTracking()
        .Then(token.ThrowIfCancellationRequested)
        .FirstOrDefaultAsync(account => account.EmailNormalized == email, token)
        .Then(token.ThrowIfCancellationRequested)
        .Map(ToDomain);

    public async Task CreateAccount(Domain.Account account, CancellationToken token) => await account
        .Map(ToData)
        .Then(data => db.Accounts.Add(data))
        .Then(() => db.SaveChangesAsync(token))
        .Then(token.ThrowIfCancellationRequested);

    private static Models.Account ToData(Domain.Account domain) => new() {
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

    private static Domain.Account? ToDomain(Models.Account? account) =>
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
