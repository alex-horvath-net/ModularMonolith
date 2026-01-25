namespace Experts.SecurityOfficer.CreateVisitor;

public class BlazorClient(UserStory userStory) {
    public Response Run(Request loginRequest) { 
        var userStoryRequest = new UserStory.Request();
        var userStoryResponse = userStory.Run(userStoryRequest);
        return new Response(userStoryResponse.IsUserStoryEnabled);
    }

    public record Request();
    public record Response(bool IsUserStoryEnabled);
}
