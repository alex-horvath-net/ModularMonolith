using Experts.SecurityOfficer.Shared.Domain;

namespace Experts.SecurityOfficer.Login;

public class UserStory(
    Authenticate authenticate,
    Authorize authorize) {
    public async Task<Response> Run(Request request, CancellationToken token) {
        var response = new Response();

        // Authenticate
        await authenticate.Run(request, response, token);
        if (response.ErrorMessage != null)
            return response;

        await authorize.Run(response, token);
        if (response.ErrorMessage != null)
            return response;

        response.IsUserStoryEnabled = true;
        return response;
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
