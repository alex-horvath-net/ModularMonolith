using Accounts.Core.Infrastructure.Data.Models;
using Core.Domain.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Accounts.Core.Infrastructure.Data.Rrepositories;

public sealed class AccountRepository(SecurityDbContext db) : IAccountRepository {
    public async Task<Domain.Account?> FindAccountByEmail(string email, CancellationToken token) => await db.Accounts
        .Include(account => account.Roles)
        .AsNoTracking()
        .Next(token.ThrowIfCancellationRequested)
        .FirstOrDefaultAsync(account => account.EmailNormalized == email, token)
        .Next(token.ThrowIfCancellationRequested)
        .Map(ToDomain);

    public async Task CreateAccount(Domain.Account account, CancellationToken token) {
        ArgumentNullException.ThrowIfNull(account);

        var accountData = account.Map(ToData);
        var dbRoles = await db.Roles.ToListAsync(token);

        accountData.Roles = (
            from role in accountData.Roles
            join dbRole in dbRoles on role.NormalizedName equals dbRole.NormalizedName into roleJoin
            from dbRole in roleJoin.DefaultIfEmpty()
            select dbRole ?? role
        ).ToHashSet();

        db.Accounts.Add(accountData);
        await db.SaveChangesAsync(token);
        token.ThrowIfCancellationRequested();
    }

    private static Account ToData(Domain.Account domain) => new() {
        Id = domain.Id,
        Email = domain.Email,
        EmailNormalized = domain.Email.ToLowerInvariant(),
        UserName = domain.UserName,
        UserNameNormalized = domain.UserName.ToLowerInvariant(),
        PasswordHash = domain.PasswordHash,
        PasswordChangedAtUtc = domain.CreatedAtUtc,
        Roles = domain.Roles.Select(ToData).ToHashSet(),
        IsLocked = domain.IsLocked,
        FailedAccessCount = 0,
        CreatedAtUtc = domain.CreatedAtUtc,
        UpdatedAtUtc = domain.CreatedAtUtc,
        IsDeleted = false
    };

    private static Role ToData(string role) => new() {
        Name = role,
        NormalizedName = role.ToLowerInvariant()
    };

    private static Domain.Account? ToDomain(Account? account) =>
        account is null ? null : new(
            account.Id,
            account.Email,
            account.UserName,
            account.PasswordHash,
            account.Roles.Select(MapToDomain).ToHashSet(),
            account.IsLocked,
            account.CreatedAtUtc);

    private static string MapToDomain(Role role) =>
        role.Name;
}
