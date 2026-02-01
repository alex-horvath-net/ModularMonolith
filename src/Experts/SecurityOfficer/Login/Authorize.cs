using Data = Experts.SecurityOfficer.Common.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Common.Domain;

namespace Experts.SecurityOfficer.Login;

public class Authorize(Authorize.IStore store) {
    private readonly IStore store = store ?? throw new ArgumentNullException(nameof(store));
    public async Task Run(UserStory.State state) {
        if (state.Account is null) {
            state.Response.ErrorMessage = "Account not found";
            return;
        }

        state.Response.Roles = state.Account.Roles.ToList();
    }

    public interface IStore {
        Task<Domain.Account?> FindById(Guid id, CancellationToken token);
    }

    public class Store(Data.SecurityOfficerDbContext db) : IStore {
        public async Task<Domain.Account?> FindById(Guid id, CancellationToken token) {
            var dataAccount = await db.Accounts.FindAsync([id], token);
            if (dataAccount == null)
                return null;
            // Map Infrastructure.Data.Models.Account to Domain.Account
            return new Domain.Account(
                dataAccount.Id,
                dataAccount.Email,
                dataAccount.UserName,
                dataAccount.PasswordHash,
                ParseRoles(dataAccount.Roles),
                dataAccount.IsLocked,
                dataAccount.CreatedAtUtc);
        }

        private static IReadOnlySet<string> ParseRoles(IEnumerable<Data.Models.Role> roles) {
            if (roles is null) {
                return new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            return new HashSet<string>(
                roles
                .Select(r => r.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name)),
                StringComparer.OrdinalIgnoreCase);
        }
    }
}