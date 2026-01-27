using Experts.SecurityOfficer.Shared.Domain;
using static Experts.SecurityOfficer.Login.UserStory;

namespace Experts.SecurityOfficer.Login;

public class UserStoryBlazorClient(UserStory userStory) {
    public async Task<ClientResponse> Run(ClientRequest clientRequest) {
        ArgumentNullException.ThrowIfNull(clientRequest);

        var userStoryRequest = GetUserStoryRequest(clientRequest);
        var userStoryResponse = await userStory.Run(userStoryRequest);
        return GetClientResponse(userStoryResponse);
    }

    private UserStory.Request GetUserStoryRequest(ClientRequest clientRequest) => new(
        clientRequest.ApplicationUser.Identity.VisitorId,
        AccountType.LocalAccount,
        new Dictionary<string, string>() {
            { "Email" , clientRequest.Email },
            { "Password" , clientRequest.Password }
        });


    private ClientResponse GetClientResponse(UserStory.Response response) => new(
        response.IsUserStoryEnabled);

    public class ClientRequest {
        public ClientRequest(ApplicationUser applicationUser) {
            ApplicationUser = applicationUser ?? throw new ArgumentNullException(nameof(applicationUser));
        }

        public ApplicationUser ApplicationUser { get; }

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public record ClientResponse(bool IsUserStoryEnabled);
}
