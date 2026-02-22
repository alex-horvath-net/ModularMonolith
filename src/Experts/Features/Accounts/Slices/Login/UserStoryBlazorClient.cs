using Business.Features.Accounts.Domain;

namespace Business.Features.Accounts.Slices.Login;

public interface IUserStoryBlazorClient {
    Task<UserStoryBlazorClientResponse> Run(UserStoryBlazorClientRequest clientRequest);
}

internal class UserStoryBlazorClient(UserStory userStory) : IUserStoryBlazorClient {
    public async Task<UserStoryBlazorClientResponse> Run(UserStoryBlazorClientRequest clientRequest) {
        var userStoryRequest = GetUserStoryRequest(clientRequest);
        var userStoryResponse = await userStory.Run(userStoryRequest, CancellationToken.None);
        var clientResponse = GetClientResponse(userStoryResponse);
        return clientResponse;
    }

    private UserStoryRequest GetUserStoryRequest(UserStoryBlazorClientRequest clientRequest) => new(
        clientRequest.ApplicationUser.Identity.VisitorId,
        AccountType.LocalAccount,
        new Dictionary<string, string>() {
            { "Email" , clientRequest.Email },
            { "Password" , clientRequest.Password }
        });
    private UserStoryBlazorClientResponse GetClientResponse(UserStoryResponse response) => new(
        response.ErrorMessage,
        response.AuthenticationId,
        response.UserName,
        response.Roles);
}

public class UserStoryBlazorClientRequest(ApplicationUser applicationUser) {
    public ApplicationUser ApplicationUser { get; } = applicationUser ?? throw new ArgumentNullException(nameof(applicationUser));

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public record UserStoryBlazorClientResponse(
    string? ErrorMessage,
    Guid? AuthenticationId,
    string? UserName,
    List<string> Roles);
