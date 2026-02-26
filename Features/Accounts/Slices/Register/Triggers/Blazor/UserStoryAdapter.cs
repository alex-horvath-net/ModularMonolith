using Common.Tasks;
using Features.Accounts.Slices.Register.UserStory;
namespace Features.Accounts.Slices.Register.Triggers.Blazor;

internal sealed class UserStoryAdapter(UserStory.UserStory userStory) : IRegister {

    public Task<RegisterResponse> Run(RegisterRequest request, CancellationToken token = default) => userStory
        .Register(request.Map(ToUserStoryRequest), token)
        .Map(ToGatewayResponse);

    private Request ToUserStoryRequest(RegisterRequest gatewayRequest) => new(
        gatewayRequest.Email,
        gatewayRequest.UserName,
        gatewayRequest.Password,
        gatewayRequest.Roles.ToArray());

    private RegisterResponse ToGatewayResponse(Response userStoryResponse) => new(
        userStoryResponse.AccountId,
        userStoryResponse.Email!,
        userStoryResponse.Roles);
}
