using Experts.SecurityOfficer.Shared.Security;
using Microsoft.EntityFrameworkCore;
using Data = Experts.SecurityOfficer.Shared.Infrastructure.Data;
using Domain = Experts.SecurityOfficer.Shared.Domain;

namespace Experts.SecurityOfficer.Login;

public class Authenticate(
    Authenticate.IStore store,
    IPasswordHasher hasher) {
    public async Task Run(
        UserStory.Request request,
        UserStory.Response response,
        CancellationToken token) {

        if (request.AccountType != UserStory.AccountType.LocalAccount) {
            response.ErrorMessage = "Account type not found";
            return;
        }

        await LocalAccountAuthentication(request, response, token);
        if (response.ErrorMessage != null) {
            return;
        }

        return;
    }

    private async Task LocalAccountAuthentication(UserStory.Request request, UserStory.Response response, CancellationToken token) {
        if (!request.Credentials.TryGetValue("Email", out var email)) {
            response.ErrorMessage = "Credential not found. Missing Email";
            return;
        }

        if (!request.Credentials.TryGetValue("Password", out var password)) {
            response.ErrorMessage = "Credential not found. Missing Password";
            return;
        }

        var account = await store.FindByEmail(email, token);

        if (account is null) {
            response.ErrorMessage = "Account not found";
            return;
        }

        if (account.IsLocked) {
            response.ErrorMessage = "Account locked";
            return;
        }

        if (!hasher.Verify(password, account.PasswordHash)) {
            response.ErrorMessage = "Invalid password";
            return;
        }

        response.AuthenticationId = account.Id;
    }

    public interface IStore {
        Task<Domain.Account?> FindByEmail(string email, CancellationToken token);
    }

    public class Store(Data.SecurityOfficerDbContext db) : IStore {
        public async Task<Domain.Account?> FindByEmail(string email, CancellationToken token) {
            var accountInfra =
                await db
                    .Accounts
                    .Where(account => account.Email == email)
                    .FirstOrDefaultAsync(token);

            if (accountInfra is null) {
                return null;
            }

            return MapToDomain(accountInfra);
        }

        private static Domain.Account MapToDomain(Data.Models.Account accountData) => new(
            accountData.Id,
            accountData.Email,
            accountData.UserName,
            accountData.PasswordHash,
            ParseRoles(accountData.Roles),
            accountData.IsLocked,
            accountData.CreatedAtUtc);

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
