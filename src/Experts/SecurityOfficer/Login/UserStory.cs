using Experts.SecurityOfficer.Common.Domain;

namespace Experts.SecurityOfficer.Login;

public class UserStory(
    Authenticate authenticate,
    Authorize authorize) {

    public async Task<Response> Run(Request request, CancellationToken token) {
        var context = new Context(request, new Response(), token);

        if (!await authenticate.Run(context))
            return context.Response;

        if (!await authorize.Run(context))
            return context.Response;

        return context.Response;
    }

    public sealed record Context(Request Request, Response Response, CancellationToken Token) {
        internal Account? Account { get; set; }
        internal bool Failed { get; set; }
    }

    public record Request(
        Guid VisitorId,
        AccountType AccountType,
        IReadOnlyDictionary<string, string> Credentials);

    public enum AccountType {
        LocalAccount,
        AzureAccount,
        SSOAccount,
    }
    public record Response() {
        public string? ErrorMessage { get; internal set; }
        public Guid? AuthenticationId { get; internal set; }
        public string? UserName { get; internal set; }
        public List<string> Roles { get; internal set; } = [];
    }

    public enum LoginOutcome {
        Succeeded,
        Failed
    }

    public enum LoginFailureReason {
        InvalidCredentials,
        IdentityNotFound,
        IdentityLocked
    }
}
