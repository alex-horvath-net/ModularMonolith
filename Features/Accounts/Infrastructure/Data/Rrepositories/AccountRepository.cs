using Common.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Features.Accounts.Infrastructure.Data.Rrepositories;

public sealed class AccountRepository(SecurityDbContext db) : IAccountRepository {
    public async Task<Domain.Account?> FindAccountByEmail(string email, CancellationToken token) => await db.Accounts
        .AsNoTracking()
        .Then(token.ThrowIfCancellationRequested)
        .FirstOrDefaultAsync(account => account.EmailNormalized == email, token)
        .Then(token.ThrowIfCancellationRequested)
        .Map(ToDomain);

    public async Task CreateAccount(Domain.Account account, CancellationToken token) {
        ArgumentNullException.ThrowIfNull(account);

        var data = ToData(account);
        if (data.Roles.Count > 0) {
            var normalizedNames = data.Roles
                .Select(role => role.NormalizedName)
                .ToArray();

            var existingRoles = await db.Roles
                .Where(role => normalizedNames.Contains(role.NormalizedName))
                .ToListAsync(token);

            var existingRolesByName = existingRoles
                .ToDictionary(role => role.NormalizedName, StringComparer.OrdinalIgnoreCase);

            var resolvedRoles = new HashSet<Models.Role>();
            foreach (var role in data.Roles) {
                var key = role.NormalizedName;
                if (existingRolesByName.TryGetValue(key, out var existingRole)) {
                    resolvedRoles.Add(existingRole);
                } else {
                    resolvedRoles.Add(role);
                }
            }

            data.Roles = resolvedRoles;
        }

        db.Accounts.Add(data);
        await db.SaveChangesAsync(token);
        token.ThrowIfCancellationRequested();
    }

    private static Models.Account ToData(Domain.Account domain) => new() {
        Id = domain.Id,
        Email = domain.Email,
        EmailNormalized = domain.Email.ToLowerInvariant(),
        UserName = domain.UserName,
        UserNameNormalized = domain.UserName.ToLowerInvariant(),
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
        NormalizedName = role.ToLowerInvariant()
    };

    private static Domain.Account? ToDomain(Models.Account? account) =>
        account is null ? null : new(
            account.Id,
            account.Email,
            account.UserName,
            account.PasswordHash,
            account.Roles.Select(MapToDomain).ToHashSet(),
            account.IsLocked,
            account.CreatedAtUtc);

    private static string MapToDomain(Models.Role role) =>
        role.Name;
}
