using Experts.SecurityOfficer.Shared.Domain;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.IdentityModel.Tokens;
using static Experts.SecurityOfficer.Login.UserStory;

namespace Experts.SecurityOfficer.Login;

public class UserStory(
    Authenticate authenticate) {
    public async Task<Response> Run(Request request, CancellationToken token) {

        // Load existing visitor
        // Authenticate visitor based on AccountType and Credentials
        var account = 
            await authenticate.Run(
                request.AccountType,
                request.Credentials,
                token);

        return new Response(
            true,
            LoginOutcome.Failed,
            LoginFailureReason.InvalidCredentials);
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
    public record Response(
        bool IsUserStoryEnabled,
        LoginOutcome Outcome,
        LoginFailureReason? Failure);

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
