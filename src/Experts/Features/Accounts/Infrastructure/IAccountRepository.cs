using Business.Features.Accounts.Domain;

namespace Business.Features.Accounts.Infrastructure;

public interface IAccountRepository {
    Task CreateAccount(Account account, CancellationToken token);
    Task<Account?> FindAccountByEmail(string email, CancellationToken token);
}