using Experts.SecurityOfficer.Shared.Domain;

namespace Experts.SecurityOfficer.CreateVisitor;

public class UserStory {
    public async Task<Response> Run(Request request) {

        await Task.CompletedTask;
        var response = new Response();
        response = response with { IsUserStoryEnabled = true };
        response = response with { ApplicationUser = CreateApplicationUserUser(request) };
        return response;
    }

    private static ApplicationUser CreateApplicationUserUser(Request request) {
        var application = new Application(request.ApplicationName, request.ApplicationVersion);
        var identity = new Identity(request.VisitorId, DateTime.UtcNow, null, null);
        var roles = new List<string>() { "Visitor" };

        var user = new ApplicationUser(application, identity, roles);

        return user;
    }

    public record Request(
        string ApplicationName, 
        string ApplicationVersion, 
        Guid VisitorId);
    
    public record Response(
        bool IsUserStoryEnabled, 
        ApplicationUser ApplicationUser) {
        public Response() : this(false, null) { }
    }
}
