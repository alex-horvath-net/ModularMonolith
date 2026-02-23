using Core.Infrastructure;
using Features.Accounts.Domain;
using Features.Accounts.Infrastructure;
using Features.Accounts.Slices.Login.WorkSteps;

namespace Features.Accounts.Slices.Login;

internal sealed class UserStory {
    private readonly Validate validate;
    private readonly Normalize normalize;
    private readonly Authenticate authenticate;
    private readonly Authorize authorize;

    internal UserStory(IAccountRepository repository, IHasher hasher) {
        validate = new Validate();
        normalize = new Normalize();
        authenticate = new Authenticate(repository, hasher);
        authorize = new Authorize();
    }

    public async Task<UserStoryResponse> Run(UserStoryRequest request, CancellationToken token) {
        var context = new Context(request, new(), token);

        if (!await validate.Run(context))
            return context.Response;

        if (!normalize.Run(context))
            return context.Response;

        if (!await authenticate.Run(context))
            return context.Response;

        if (!await authorize.Run(context))
            return context.Response;

        return context.Response;
    }

    public sealed record Context(UserStoryRequest Request, UserStoryResponse Response, CancellationToken Token) {
        public string? Email { get; set; }
        public string? NormalizedEmail { get; set; }
        public string? Password { get; set; }
        internal Account? Account { get; set; }
    }
}

internal sealed record UserStoryRequest(
     Guid VisitorId,
     AccountType AccountType,
     IReadOnlyDictionary<string, string> Credentials);

internal enum AccountType {
    LocalAccount,
    AzureAccount,
    SSOAccount,
}

public sealed record UserStoryResponse() {
    public string? ErrorMessage { get; internal set; }
    public Guid? AuthenticationId { get; internal set; }
    public string? UserName { get; internal set; }
    public List<string> Roles { get; internal set; } = [];
}

public static class UserStoryConstants {
    public const string AccountTypeNotFound = "Account type not found";
    public const string MissingEmail = "Credential not found. Missing Email";
    public const string MissingPassword = "Credential not found. Missing Password";
    public const string AccountNotFound = "Account not found";
    public const string AccontLocked = "Account locked";
    public const string InvalidPassword = "Invalid password";
    public const string Email = "Email";
    public const string Password = "Password";
}
