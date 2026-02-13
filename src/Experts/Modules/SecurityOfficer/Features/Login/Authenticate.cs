using Business.Modules.SecurityOfficer.Domain;
using Business.Modules.SecurityOfficer.Infrastructure.Data;
using Business.Modules.SecurityOfficer.Infrastructure.Data.Models;
using Business.Modules.SecurityOfficer.Infrastructure.Hash;
using Business.Modules.SecurityOfficer.Infrastructure.Random;
using Common.Tasks;
using Microsoft.EntityFrameworkCore;
using Data2 = Business.Modules.SecurityOfficer.Infrastructure.Data;
using Data = Experts.SecurityOfficer.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Common.Domain;

namespace Business.Modules.SecurityOfficer.Features.Login;

internal sealed class Authenticate {
    private readonly IAuthenticateStore store;
    private readonly Pbkdf2HashGenerator hasher;
    internal Authenticate(IAuthenticateStore store, IRandom random) {
        this.store = store;
        hasher = new Pbkdf2HashGenerator(random);
    }

    public async Task<bool> Run(UserStory.Context context) {

        if (context.Request.AccountType != AccountType.LocalAccount) {
            context.Response.ErrorMessage = UserStoryConstants.AccountTypeNotFound;
            return false;
        }

        if (!context.Request.Credentials.TryGetValue(UserStoryConstants.Email, out var email)) {
            context.Response.ErrorMessage = UserStoryConstants.MissingEmail;
            return false;
        }

        if (!context.Request.Credentials.TryGetValue(UserStoryConstants.Password, out var password)) {
            context.Response.ErrorMessage = UserStoryConstants.MissingPassword;
            return false;
        }

        context.Account = await store.FindByEmail(email, context.Token);

        if (context.Account is null) {
            context.Response.ErrorMessage = UserStoryConstants.AccountNotFound;
            return false;
        }

        if (context.Account.IsLocked) {
            context.Response.ErrorMessage = UserStoryConstants.AccontLocked;
            return false;
        }

        if (!hasher.Verify(password, context.Account.PasswordHash)) {
            context.Response.ErrorMessage = UserStoryConstants.InvalidPassword;
            return false;
        }

        context.Response.AuthenticationId = context.Account.Id;
        context.Response.UserName = context.Account.UserName;

        return true;
    }
}

public interface IAuthenticateStore {
    Task<Account?> FindByEmail(string email, CancellationToken token);
}

internal sealed class AuthenticateStore(SecurityOfficerDbContext db) : IAuthenticateStore {
    public Task<Account?> FindByEmail(string email, CancellationToken token) {
        var normalizedEmail = NormalizeEmail(email);

        return db.Accounts
            .Include(account => account.Roles)
            .Where(account => account.EmailNormalized == normalizedEmail)
            .FirstOrDefaultAsync(token)
            .Then(AccountMapper.ToDomain);
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}
