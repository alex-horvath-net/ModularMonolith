using Business.Modules.SecurityOfficer.Infrastructure.Data;

namespace Business.Modules.SecurityOfficer.Features.Register.Infrastructure;

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
