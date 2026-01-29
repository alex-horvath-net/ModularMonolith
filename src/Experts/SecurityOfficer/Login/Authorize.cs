using System;
using System.Linq;
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
                return new Domain.Account(
                    dataAccount.Id,
                    dataAccount.Email,
                    dataAccount.UserName,
                    dataAccount.PasswordHash,
                    ParseRoles(dataAccount.Roles),
                    dataAccount.IsLocked,
                    dataAccount.CreatedAtUtc);
            }

            private static IReadOnlyCollection<string> ParseRoles(string? rawRoles) {
                if (string.IsNullOrWhiteSpace(rawRoles)) {
                    return Array.Empty<string>();
                }

                return rawRoles
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
        }
    }
}