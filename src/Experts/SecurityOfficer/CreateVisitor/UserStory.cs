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
        var identity = new Identity(request.VisitorId, DateTime.UtcNow, string.Empty, string.Empty);
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
        private static readonly ApplicationUser EmptyApplicationUser = new(
            new Application(string.Empty, string.Empty),
            new Identity(Guid.Empty, DateTime.MinValue, string.Empty, string.Empty),
            Array.Empty<string>());

        public Response() : this(false, EmptyApplicationUser) { }
    }
}
