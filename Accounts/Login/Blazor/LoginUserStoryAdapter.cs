using Accounts.Login;
using Core.Domain.Tasks;

namespace Accounts.Login.Blazor;

internal sealed class LoginUserStoryAdapter(UserStory userStory) : ILogin {
    public async Task<LoginResponse> Run(LoginRequest gatewayRequest, CancellationToken token = default) {
        var userStoryRequest = gatewayRequest.Map(ToUserStoryRequest);
        var userStoryResponse = await userStory.Run(userStoryRequest, token);
        var gatrewayResponse = userStoryResponse.Map(ToGatewayResponse);
        return gatrewayResponse;
    }

    private UserStoryRequest ToUserStoryRequest(LoginRequest clientRequest) => new(
        clientRequest.ApplicationUser.Identity.VisitorId,
        AccountType.LocalAccount,
        new Dictionary<string, string>() {
            { "Email" , clientRequest.Email },
            { "Password" , clientRequest.Password }
        });
    private LoginResponse ToGatewayResponse(UserStoryResponse response) => new(
        response.ErrorMessage,
        response.AuthenticationId,
        response.UserName,
        response.Roles);
}
