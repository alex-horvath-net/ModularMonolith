using Experts.SecurityOfficer.Common.Domain;
using Experts.SecurityOfficer.Common.Infrastructure.Random;

namespace Experts.SecurityOfficer.Register;

internal sealed class UserStory {
    private readonly Create create;

    internal UserStory(ICreateAccountStore store, IRandom random, ICreateClock clock) {
        create = new Create(store, random, clock);
    }

    public async Task<UserStoryResponse> Register(UserStoryRequest request, CancellationToken token) {
        var context = new UserStoryContext(request, new(), token);
        //Crate

        if (!await create.Run(context)) {
            return context.Response;
        }

        //Activate email
        //Activate MFA

        return context.Response;
    }

    public sealed record UserStoryContext(UserStoryRequest Request, UserStoryResponse Response, CancellationToken Token) {
        public UserStoryRequest? NormalizedRequest { get; set; }
        public Account? MathingAccount { get; internal set; }
        public Account? Account { get; internal set; }
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
    public string? Email { get; internal set; }
    public string? UserName { get; internal set; }
    public IReadOnlyCollection<string>? Roles { get; internal set; }

}

public static class UserStoryConstants {
    public const string RequestCanNotBeNell = "Request can not be null";
    public const string EmailIsRequired = "Email is required";
    public const string PasswordMutBeContain = "Password must be at least 12 characters and contain upper, lower, digit, and symbol";
    public const string UserNameIsRequired = "UserName is required";
    public const string AtLeastOneRoleRequired = "At least one role is required";
    public const string AccountAlreadyExists = "Account with the same email already exists";
}
