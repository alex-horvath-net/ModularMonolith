using Experts.SecurityOfficer.Shared.Domain;

namespace Experts.SecurityOfficer.CreateVisitor;

public class UserStory {
    public async Task<Response> Run(Request request) {

        var user = new ApplicationUser(
            new Application(request.Name, request.Version),
            new Identity(request.VisitorId, DateTime.UtcNow, null, null),
            ["Visitor"]);

        await Task.CompletedTask;
        return new Response(true, user);
    }

    public record Request(string Name, string Version, Guid VisitorId);
    public record Response(
        bool IsUserStoryEnabled,
        ApplicationUser User);
}
