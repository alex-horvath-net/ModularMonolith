using Common.Tasks;
using Experts.SecurityOfficer.Common.Infrastructure.Cryptography;
using Microsoft.EntityFrameworkCore;
using Data = Experts.SecurityOfficer.Common.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Common.Domain;

namespace Experts.SecurityOfficer.Login;

internal sealed class Authenticate {
    private readonly IAuthenticateStore store;
    private readonly Pbkdf2PasswordHasher hasher;
    internal Authenticate(IAuthenticateStore store, IRandomNumberGenerator random) {
        this.store = store;
        hasher = new Pbkdf2PasswordHasher(random);
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
    Task<Domain.Account?> FindByEmail(string email, CancellationToken token);
}

internal sealed class AuthenticateStore(Data.SecurityOfficerDbContext db) : IAuthenticateStore {
    public Task<Domain.Account?> FindByEmail(string email, CancellationToken token) {
        var normalizedEmail = NormalizeEmail(email);

        return db.Accounts
            .Include(account => account.Roles)
            .Where(account => account.EmailNormalized == normalizedEmail)
            .FirstOrDefaultAsync(token)
            .Then(Data.Models.AccountMapper.ToDomain);
    }

    private static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}
