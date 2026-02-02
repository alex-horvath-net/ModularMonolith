using Experts.SecurityOfficer.Common.Security;
using Microsoft.EntityFrameworkCore;
using Domain = Experts.SecurityOfficer.Common.Domain;
using Data = Experts.SecurityOfficer.Common.Infrastructure.Data;

namespace Experts.SecurityOfficer.Login;

public class Authenticate(
    Authenticate.IStore store,
    IPasswordHasher hasher) {
    public async Task Run(UserStory.Context context) {

        if (context.Request.AccountType != UserStory.AccountType.LocalAccount) {
            context.Response.ErrorMessage = "Account type not found";
            return;
        }

        if (!context.Request.Credentials.TryGetValue("Email", out var email)) {
            context.Response.ErrorMessage = "Credential not found. Missing Email";
            return;
        }

        if (!context.Request.Credentials.TryGetValue("Password", out var password)) {
            context.Response.ErrorMessage = "Credential not found. Missing Password";
            return;
        }

        context.Account = await store.FindByEmail(email, context.Token);

        if (context.Account is null) {
            context.Response.ErrorMessage = "Account not found";
            return;
        }

        if (context.Account.IsLocked) {
            context.Response.ErrorMessage = "Account locked";
            return;
        }

        if (!hasher.Verify(password, context.Account.PasswordHash)) {
            context.Response.ErrorMessage = "Invalid password";
            return;
        }

        context.Response.AuthenticationId = context.Account.Id;

        return;
    }

    public interface IStore {
        Task<Domain.Account?> FindByEmail(string email, CancellationToken token);
    }

    public class Store(Data.SecurityOfficerDbContext db) : IStore {
        public async Task<Domain.Account?> FindByEmail(string email, CancellationToken token) {
            var accountInfra =
                await db
                    .Accounts
                    .Include(account => account.Roles)
                    .Where(account => account.Email == email)
                    .FirstOrDefaultAsync(token);

            return MapToDomain(accountInfra);
        }

        private static Domain.Account? MapToDomain(Data.Models.Account? data) => data == null ? null : new(
            data.Id,
            data.Email,
            data.UserName,
            data.PasswordHash,
            data.Roles.Select(MapToDomain).ToHashSet(),
            data.IsLocked,
            data.CreatedAtUtc);

        private static string MapToDomain(Data.Models.Role data) =>
            data.Name;
    }
}
