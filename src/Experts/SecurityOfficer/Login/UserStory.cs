namespace Experts.SecurityOfficer.Login;

public class UserStory {
    public async Task<Response> Run(Request request) {
        await Task.CompletedTask;
        return new Response(true);
    }

    public record Request(
        Guid VisitorId,
        AccountType AccountType,
        IReadOnlyDictionary<string, string> Credentials);
    
    public enum AccountType {
        LocalAccount,
        AzureAccount,
        FacebookAccount,
        SSOAccount,
    }
    public record Response(
        bool IsUserStoryEnabled);
}
