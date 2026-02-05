using Common.Tasks;
using Experts.SecurityOfficer.Common.Security;
using Microsoft.EntityFrameworkCore;
using Data = Experts.SecurityOfficer.Common.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Common.Domain;

namespace Experts.SecurityOfficer.Login;

public static class AuthenticateConstants {
    public const string AccountTypeNotFound = "Account type not found";
    public const string MissingEmail = "Credential not found. Missing Email";
    public const string MissingPassword = "Credential not found. Missing Password";
    public const string AccountNotFound = "Account not found";
    public const string AccontLocked = "Account locked";
    public const string InvalidPassword = "Invalid password";
}

public static class LocalAccountConstants {
    public const string Email = "Email";
    public const string Password = "Password";
}


public class Authenticate(
    Authenticate.IStore store,
    IPasswordHasher hasher) {
    public async Task<bool> Run(UserStory.Context context) {

        if (context.Request.AccountType != UserStory.AccountType.LocalAccount) {
            context.Response.ErrorMessage = AuthenticateConstants.AccountTypeNotFound;
            return false;
        }

        if (!context.Request.Credentials.TryGetValue(LocalAccountConstants.Email, out var email)) {
            context.Response.ErrorMessage = AuthenticateConstants.MissingEmail;
            return false;
        }

        if (!context.Request.Credentials.TryGetValue(LocalAccountConstants.Password, out var password)) {
            context.Response.ErrorMessage = AuthenticateConstants.MissingPassword;
            return false;
        }

        context.Account = await store.FindByEmail(email, context.Token);

        if (context.Account is null) {
            context.Response.ErrorMessage = AuthenticateConstants.AccountNotFound;
            return false;
        }

        if (context.Account.IsLocked) {
            context.Response.ErrorMessage = AuthenticateConstants.AccontLocked;
            return false;
        }

        if (!hasher.Verify(password, context.Account.PasswordHash)) {
            context.Response.ErrorMessage = AuthenticateConstants.InvalidPassword;
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
