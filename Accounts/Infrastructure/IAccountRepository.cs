using Accounts.Domain;

namespace Accounts.Infrastructure;

public interface IAccountRepository {
    Task CreateAccount(Account account, CancellationToken token);
    Task<Account?> FindAccountByEmail(string email, CancellationToken token);
}