using Experts.SecurityOfficer.Common.Domain;

namespace Experts.SecurityOfficer.Login;

public class UserStory(
    Authenticate authenticate,
    Authorize authorize) {

    private State? state;
    public async Task<Response> Run(Request request, CancellationToken token) {
        state = new State(request, new Response(), token);

        // Authenticate
        await authenticate.Run(state);
        if (state.Response.ErrorMessage != null)
            return state.Response;

        await authorize.Run(state);
        if (state.Response.ErrorMessage != null)
            return state.Response;

        state.Response.IsUserStoryEnabled = true;
        return state.Response;
    }

    public sealed record State(Request Request, Response Response, CancellationToken Token) {
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
