using Data = Experts.SecurityOfficer.Common.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Common.Domain;
using Common.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Experts.SecurityOfficer.Login;

public class Authorize() {
    public async Task<bool> Run(UserStory.Context context) {
        if (context.Account is null) {
            context.Response.ErrorMessage = "Account not found";
            return false;
        }

        context.Response.Roles = context.Account.Roles.ToList();

        await Task.CompletedTask;

        return true;
    }

    public interface IStore {
        Task<Domain.Account?> FindById(Guid id, CancellationToken token);
    }

    public class Store(Data.SecurityOfficerDbContext db) : IStore {
        public Task<Domain.Account?> FindById(Guid id, CancellationToken token) => db.Accounts
            .FirstOrDefaultAsync(x => x.Id == id, token)
            .Then(ToDomain);

        private static Domain.Account? ToDomain(Data.Models.Account? dataAccount) {
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