using Business.Experts.SecurityOfficer.Domain;

namespace Business.Experts.SecurityOfficer.Infrastructure;
public interface IAccountRepository {
    Task CreateAccount(Account account, CancellationToken token);
    Task<Account?> FindAccountByEmail(string email, CancellationToken token);
}