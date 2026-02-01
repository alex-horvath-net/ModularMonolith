using Experts.SecurityOfficer.Common.Domain;
using static Experts.SecurityOfficer.Login.UserStory;

namespace Experts.SecurityOfficer.Login;

public class UserStoryBlazorClient(UserStory userStory) {
    public async Task<ClientResponse> Run(ClientRequest clientRequest) {
        ArgumentNullException.ThrowIfNull(clientRequest);

        var userStoryRequest = GetUserStoryRequest(clientRequest);
        var userStoryResponse = await userStory.Run(userStoryRequest, CancellationToken.None);
        return GetClientResponse(userStoryResponse);
    }

    private Request GetUserStoryRequest(ClientRequest clientRequest) => new(
        clientRequest.ApplicationUser.Identity.VisitorId,
        AccountType.LocalAccount,
        new Dictionary<string, string>() {
            { "Email" , clientRequest.Email },
            { "Password" , clientRequest.Password }
        });


    private ClientResponse GetClientResponse(Response response) => new(
        response.IsUserStoryEnabled);

    public class ClientRequest(ApplicationUser applicationUser) {
        public ApplicationUser ApplicationUser { get; } = applicationUser ?? throw new ArgumentNullException(nameof(applicationUser));

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }

    public record ClientResponse(bool IsUserStoryEnabled);
}
