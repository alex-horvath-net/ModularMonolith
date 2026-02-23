using Core.Features.Accounts.Domain;

namespace Core.Features.Accounts.Infrastructure;

public interface IAccountRepository {
    Task CreateAccount(Account account, CancellationToken token);
    Task<Account?> FindAccountByEmail(string email, CancellationToken token);
}