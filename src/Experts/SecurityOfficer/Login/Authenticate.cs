using Common.Tasks;
using Experts.SecurityOfficer.Common.Security;
using Microsoft.EntityFrameworkCore;
using Data = Experts.SecurityOfficer.Common.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Common.Domain;

namespace Experts.SecurityOfficer.Login;

public class Authenticate(
    Authenticate.IStore store,
    IPasswordHasher hasher) {

    public async Task<bool> Run(UserStory.Context context) {

        if (context.Request.AccountType != UserStory.AccountType.LocalAccount) {
            context.Response.ErrorMessage = "Account type not found";
            return false;
        }

        if (!context.Request.Credentials.TryGetValue("Email", out var email)) {
            context.Response.ErrorMessage = "Credential not found. Missing Email";
            return false;
        }

        if (!context.Request.Credentials.TryGetValue("Password", out var password)) {
            context.Response.ErrorMessage = "Credential not found. Missing Password";
            return false;
        }

        context.Account = await store.FindByEmail(email, context.Token);

        if (context.Account is null) {
            context.Response.ErrorMessage = "Account not found";
            return false;
        }

        if (context.Account.IsLocked) {
            context.Response.ErrorMessage = "Account locked";
            return false;
        }

        if (!hasher.Verify(password, context.Account.PasswordHash)) {
            context.Response.ErrorMessage = "Invalid password";
            return false;
        }

        context.Response.AuthenticationId = context.Account.Id;
        context.Response.UserName = context.Account.UserName;

        return true;
    }

    public interface IStore {
        Task<Domain.Account?> FindByEmail(string email, CancellationToken token);
    }

    public class Store(Data.SecurityOfficerDbContext db) : IStore {
        public Task<Domain.Account?> FindByEmail(string email, CancellationToken token) => db.Accounts
            .Include(account => account.Roles)
            .Where(account => account.Email == email)
            .FirstOrDefaultAsync(token)
            .Then(Data.Models.AccountMapper.ToDomain);
    }
}
