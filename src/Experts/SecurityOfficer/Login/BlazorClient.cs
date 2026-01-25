namespace Experts.SecurityOfficer.Login;

public class BlazorClient {
    public Response Run(Request loginRequest) {
        var userStory = new UserStory();
        var userStoryRequest = new UserStory.Request();
        var userStoryResponse = userStory.Run(userStoryRequest);
        return new Response(userStoryResponse.IsUserStoryEnabled);
    }

    public record Request(Shared.Domain.ApplicationUser User);
    public record Response(bool IsUserStoryEnabled);
}
