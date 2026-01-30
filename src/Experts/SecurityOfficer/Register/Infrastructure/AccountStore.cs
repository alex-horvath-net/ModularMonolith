using Experts.SecurityOfficer.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Data = Experts.SecurityOfficer.Shared.Infrastructure.Data;

namespace Experts.SecurityOfficer.Register.Infrastructure;

public sealed class AccountStore(Data.SecurityOfficerDbContext db) : UserStory.IAccountStore
{
    public async Task<Account?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(normalizedEmail);

        var entity = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Email == normalizedEmail, cancellationToken)
            .ConfigureAwait(false);

        return entity is null ? null : Map(entity);
    }

    public async Task CreateAsync(Account account, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(account);

        var entity = new Data.Models.Account
        {
            Id = account.Id,
            Email = account.Email,
            UserName = account.UserName,
            PasswordHash = account.PasswordHash,
            IsLocked = account.IsLocked,
            Roles = string.Join(',', account.Roles),
            CreatedAtUtc = account.CreatedAtUtc
        };

        db.Accounts.Add(entity);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Account Map(Data.Models.Account entity) => new(
        entity.Id,
        entity.Email,
        entity.UserName,
        entity.PasswordHash,
        ParseRoles(entity.Roles),
        entity.IsLocked,
        entity.CreatedAtUtc);

    private static IReadOnlyCollection<string> ParseRoles(string? rawRoles)
    {
        if (string.IsNullOrWhiteSpace(rawRoles))
        {
            return Array.Empty<string>();
        }

        return rawRoles
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
