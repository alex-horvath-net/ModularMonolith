using Features.Accounts.Domain;

namespace Features.Accounts.Slices.Login;

public interface IBlazorGateway {
    Task<BlazorGatewayResponse> Run(BlazorGatewayRequest clientRequest);
}

internal sealed class BlazorGateway(UserStory userStory) : IBlazorGateway {
    public async Task<BlazorGatewayResponse> Run(BlazorGatewayRequest clientRequest) {
        var userStoryRequest = GetUserStoryRequest(clientRequest);
        var userStoryResponse = await userStory.Run(userStoryRequest, CancellationToken.None);
        var clientResponse = GetClientResponse(userStoryResponse);
        return clientResponse;
    }

    private UserStoryRequest GetUserStoryRequest(BlazorGatewayRequest clientRequest) => new(
        clientRequest.ApplicationUser.Identity.VisitorId,
        AccountType.LocalAccount,
        new Dictionary<string, string>() {
            { "Email" , clientRequest.Email },
            { "Password" , clientRequest.Password }
        });
    private BlazorGatewayResponse GetClientResponse(UserStoryResponse response) => new(
        response.ErrorMessage,
        response.AuthenticationId,
        response.UserName,
        response.Roles);
}

public class BlazorGatewayRequest(ApplicationUser applicationUser) {
    public ApplicationUser ApplicationUser { get; } = applicationUser ?? throw new ArgumentNullException(nameof(applicationUser));

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public record BlazorGatewayResponse(
    string? ErrorMessage,
    Guid? AuthenticationId,
    string? UserName,
    List<string> Roles);
