using Domain = Experts.SecurityOfficer.Common.Domain;
using Data = Experts.SecurityOfficer.Common.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Experts.SecurityOfficer.Register.Infrastructure;

public sealed class AccountStore(Data.SecurityOfficerDbContext db) : UserStory.IAccountStore {
    public async Task<Domain.Account?> FindByEmailAsync(string email, CancellationToken token) {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var normalizedEmail = NormalizeEmail(email);
        var entity = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(account => account.EmailNormalized == normalizedEmail, token)
            .ConfigureAwait(false);

        return MapToDomain(entity);
    }

    public async Task CreateAsync(Domain.Account account, CancellationToken token) {
        ArgumentNullException.ThrowIfNull(account);

        var data = MapToData(account)!;

        db.Accounts.Add(data);

        await db
            .SaveChangesAsync(token)
            .ConfigureAwait(false);
    }

    private static Data.Models.Account MapToData(Domain.Account domain) => new() {
        Id = domain.Id,
        Email = domain.Email,
        EmailNormalized = NormalizeEmail(domain.Email),
        UserName = domain.UserName,
        UserNameNormalized = NormalizeUserName(domain.UserName),
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
        NormalizedName = NormalizeRoleName(role)
    };

    private static Domain.Account? MapToDomain(Data.Models.Account? entity) =>
        entity is null ? null : new(
            entity.Id,
            entity.Email,
            entity.UserName,
            entity.PasswordHash,
            entity.Roles.Select(MapToDomain).ToHashSet(),
            entity.IsLocked,
            entity.CreatedAtUtc);

    private static string MapToDomain(Data.Models.Role data) =>
        data.Name;

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();

    private static string NormalizeUserName(string userName) =>
        userName.Trim().ToUpperInvariant();

    private static string NormalizeRoleName(string roleName) =>
        roleName.Trim().ToUpperInvariant();
}
