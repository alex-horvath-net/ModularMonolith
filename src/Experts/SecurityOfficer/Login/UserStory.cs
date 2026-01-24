namespace Experts.SecurityOfficer.Login; 
public class UserStory {
    public Response Run(Request request) {
        return new Response(true);
    }

    public record Request();
    public record Response(
        bool IsUserStoryEnabled);
}
