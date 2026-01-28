using Data = Experts.SecurityOfficer.Shared.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Shared.Domain;

namespace Experts.SecurityOfficer.Login {
    public class Authorize(Authorize.IStore store) {
        public async Task Run(UserStory.Response response, CancellationToken token) {
            var account = await store.FindById(response.AuthenticationId!.Value, token);
        }

        public interface IStore {
            Task<Domain.Account?> FindById(Guid id, CancellationToken token);
        }

        public class Store(Data.SecurityOfficerDbContext db) : Authorize.IStore {
            public async Task<Domain.Account?> FindById(Guid id, CancellationToken token) {
                var dataAccount = await db.Accounts.FindAsync([id], token);
                if (dataAccount == null)
                    return null;
                // Map Infrastructure.Data.Models.Account to Domain.Account
                return new Domain.Account {
                    Id = dataAccount.Id,
                    UserName = dataAccount.UserName,
                    Email = dataAccount.Email,
                    PasswordHash = dataAccount.PasswordHash,
                    IsLocked = dataAccount.IsLocked
                };
            }
        }
    }
}