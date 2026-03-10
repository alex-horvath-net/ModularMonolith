using Core.Domain.Tasks;
using Features.Accounts.Domain;

namespace Features.Accounts.Infrastructure;

public interface IAccountRepository {
    Task CreateAccount(Account account, CancellationToken token);
    Task<Account?> FindAccountByEmail(string email, CancellationToken token);
}