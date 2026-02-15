using Business.Experts.SecurityOfficer.Domain;
using Data = Business.Experts.SecurityOfficer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Business.Experts.SecurityOfficer.Infrastructure.Clock;

namespace Business.Experts.SecurityOfficer.UserStories.Register.Infrastructure;

public sealed class AccountRepository(Data.SecurityOfficerDbContext db) : WorkSteps.PreventDuplication.IRepository {
    public async Task<Account?> FindAccountByEmail(string email, CancellationToken token) {

        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(account => account.EmailNormalized == email, token);

        return MapToDomain(account);
    }

    public async Task CreateAccount(Account account, CancellationToken token) {

        var data = MapToData(account)!;

        db.Accounts.Add(data);

        await db
            .SaveChangesAsync(token);
    }

    private static Data.Models.Account MapToData(Account domain) => new() {
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

    private static Data.Models.Role MapToData(string role) => new() {
        Name = role,
        NormalizedName = role
    };

    private static Account? MapToDomain(Data.Models.Account? account) =>
        account is null ? null : new(
            account.Id,
            account.EmailNormalized,
            account.UserNameNormalized,
            account.PasswordHash,
            account.Roles.Select(MapToDomain).ToHashSet(),
            account.IsLocked,
            account.CreatedAtUtc);

    private static string MapToDomain(Data.Models.Role role) =>
        role.Name;
}

public sealed class CreateClock : SystemClock, ICreateClock { }

