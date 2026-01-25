namespace Experts.SecurityOfficer.Login;

public class UserStory {
    public async Task<Response> Run(Request request) {
        await Task.CompletedTask;
        return new Response(true);
    }

    public record Request();
    public record Response(
        bool IsUserStoryEnabled);
}
