using Experts.SecurityOfficer.Shared.Domain;
using static Experts.SecurityOfficer.Login.UserStory;

namespace Experts.SecurityOfficer.Login;

public class UserStoryBlazorClient(UserStory userStory) {
    public async Task<ClientResponse> Run(ClientRequest clientRequest) {
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

    public record ClientRequest(
        ApplicationUser ApplicationUser,
        string Email,
        string Password);

    public record ClientResponse(bool IsUserStoryEnabled);
}
