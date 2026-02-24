using Core.Infrastructure;
using Features.Accounts.Domain;
using Features.Accounts.Infrastructure;
using Features.Accounts.Slices.Register.WorkSteps;

namespace Features.Accounts.Slices.Register;

internal sealed class UserStory {
    private readonly Validate validate;
    private readonly Normalize normalize;
    private readonly PreventDuplication preventDuplication;
    private readonly Create create;
    private readonly Save save;

    internal UserStory(IAccountRepository repository, IHasher hasher, IClock clock) {
        validate = new Validate();
        normalize = new Normalize();
        preventDuplication = new PreventDuplication(repository);
        create = new Create(hasher, clock);
        save = new Save(repository);
    }

    public async Task<UserStoryResponse> Register(UserStoryRequest request, CancellationToken token) {
        var context = new UserStoryContext(request, token);

        validate.Run(context);
        normalize.Run(context);
        await preventDuplication.Run(context);
        create.Run(context);
        await save.Run(context);

        //Activate email
        //Activate MFA

        return context.ToResponse();
    }

    public sealed record UserStoryContext(UserStoryRequest Request, CancellationToken Token) {
        public UserStoryRequest? NormalizedRequest { get; set; }
        public Account? MachingAccount { get; internal set; }
        public Account? Account { get; internal set; }
        public UserStoryResponse? Response { get; internal set; }

        internal UserStoryResponse ToResponse() => new(
            ErrorMessage: null,
            Account!.Id,
            Account.Email,
            Account.UserName,
            Account.Roles);
    }
}

internal sealed record UserStoryRequest(
    string Email,
    string UserName,
    string Password,
    IReadOnlyCollection<string> Roles);

internal sealed record UserStoryResponse(
    string? ErrorMessage,
    Guid AccountId,
    string? Email,
    string? UserName,
    IReadOnlyCollection<string> Roles);

internal static class UserStoryConstants {
    public const string RequestCanNotBeNell = "Request can not be null";
    public const string EmailIsRequired = "Email is required";
    public const string PasswordMutBeContain = "Password must be at least 12 characters and contain upper, lower, digit, and symbol";
    public const string UserNameIsRequired = "UserName is required";
    public const string AtLeastOneRoleRequired = "At least one role is required";
    public const string AccountAlreadyExists = "Account with the same email already exists";
}
