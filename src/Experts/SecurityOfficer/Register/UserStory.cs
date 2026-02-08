using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Infrastructure.Security;
using Experts.SecurityOfficer.Register.Infrastructure;

namespace Experts.SecurityOfficer.Register;

internal sealed class UserStory {
    private readonly IAccountStore store;
    private readonly Pbkdf2PasswordHasher hasher;
    private readonly DefaultRolePolicy rolePolicy = new();
    private readonly IClock clock;
    private readonly Create create;

    internal UserStory(IAccountStore store, IRandomNumberGenerator random, IClock clock) {
        this.store = store ?? throw new ArgumentNullException(nameof(store));
        hasher = new(random ?? throw new ArgumentNullException(nameof(random)));
        this.clock = clock ?? throw new ArgumentNullException(nameof(clock));

        create = new Create(store, random, clock);
    }

    public async Task<UserStoryResponse> Register(UserStoryRequest request, CancellationToken token) {
        var context = new Context(request, new(), token);
        //Crate

        if (await !create.Run(context)) {
            return context.Response;
        }

        //Activate email
        //Activate MFA
    }


    public interface IAccountStore {
        Task<Account?> FindByEmailAsync(string email, CancellationToken token);
        Task CreateAsync(Account account, CancellationToken token);
    }

    public interface IRolePolicy {
        bool AreEligible(IEnumerable<string> requestedRoles);
    }

    public interface IClock {
        DateTime UtcNow { get; }
    }

    internal sealed record Context(UserStoryRequest Request, UserStoryResponse Response, CancellationToken Token) {
        internal Account? ManchingAccount { get; set; }
    }
}

public sealed record UserStoryRequest(
    string Email,
    string UserName,
    string Password,
    IReadOnlyCollection<string> Roles);

public sealed class UserStoryResponse {
    public string? ErrorMessage { get; internal set; }
    public Guid AccountId { get; internal set; }
    public string Email { get; internal set; }
    public string UserName { get; internal set; }
    public IReadOnlyCollection<string> Roles { get; internal set; }

}

public static class UserStoryConstants {
    public const string RequestCanNotBeNell = "Request can not be null";
    public const string EmailIsRequired = "Email is required";
    public const string PasswordMutBeContain = "Password must be at least 12 characters and contain upper, lower, digit, and symbol";
    public const string UserNameIsRequired = "UserName is required";
    public const string AtLeastOneRoleRequired = "At least one role is required";
}
