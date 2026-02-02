using Experts.SecurityOfficer.Common.Domain;

namespace Experts.SecurityOfficer.Login;

public class UserStory(
    Authenticate authenticate,
    Authorize authorize) {

    private Context? context;
    public async Task<Response> Run(Request request, CancellationToken token) {
        context = new Context(request, new Response(), token);

        await authenticate.Run(context);
        if (context.Response.ErrorMessage != null)
            return context.Response;

        await authorize.Run(context);
        if (context.Response.ErrorMessage != null)
            return context.Response;

        context.Response.IsUserStoryEnabled = true;
        return context.Response;
    }

    public sealed record Context(Request Request, Response Response, CancellationToken Token) {
        internal Account? Account { get; set; }
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
        public Account? Account { get; internal set; }
        public string? ErrorMessage { get; internal set; }
        public Guid? AuthenticationId { get; internal set; }
        public string? UserName { get; internal set; }
        public List<string> Roles { get; internal set; } = [];
        public bool IsUserStoryEnabled { get; internal set; }
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
