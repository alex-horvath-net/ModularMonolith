using Business.Features.Accounts.Domain;
using Business.Features.Accounts.Infrastructure;
using Business.Features.Accounts.Slices.Register.WorkSteps;

namespace Business.Features.Accounts.Slices.Register;

internal sealed class UserStory {
    private readonly Validate validate;
    private readonly Normalize normalize;
    private readonly PreventDuplication preventDuplication;
    private readonly Create create;
    private readonly Save save;
    private readonly BuildResponse buildresponse;

    internal UserStory(IAccountRepository repository, IHasher hasher, IClock clock) {
        validate = new Validate();
        normalize = new Normalize();
        preventDuplication = new PreventDuplication(repository);
        create = new Create(hasher, clock);
        save = new Save(repository);
        buildresponse = new BuildResponse();

    }

    public async Task<UserStoryResponse> Register(UserStoryRequest request, CancellationToken token) {
        var context = new UserStoryContext(request, new(), token);

        if (!validate.Run(context))
            return context.Response;

        if (!normalize.Run(context))
            return context.Response;

        if (await preventDuplication.Run(context))
            return context.Response;

        if (!create.Run(context))
            return context.Response;

        if (!await save.Run(context))
            return context.Response;

        if (!buildresponse.Run(context))
            return context.Response;

        //Activate email
        //Activate MFA

        return context.Response;
    }

    public sealed record UserStoryContext(UserStoryRequest Request, UserStoryResponse Response, CancellationToken Token) {
        public UserStoryRequest? NormalizedRequest { get; set; }
        public Account? MachingAccount { get; internal set; }
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
