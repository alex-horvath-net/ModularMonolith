using Accounts.Register.UserStory;
using Core.Domain.Tasks;
namespace Accounts.Register.Triggers.Blazor;

internal sealed class UserStoryAdapter(UserStory.UserStory userStory) : IRegister {

    public Task<RegisterResponse> Run(RegisterRequest blazorRequest, CancellationToken token = default) => userStory
        .Register(blazorRequest.Map(ToUserStoryRequest), token)
        .Map(ToBlazorResponse);

    private Request ToUserStoryRequest(RegisterRequest blazorRequest) => new(
        Email: blazorRequest.Email,
        UserName: blazorRequest.UserName,
        Password: blazorRequest.Password,
        Roles: blazorRequest.Roles.ToArray());

    private RegisterResponse ToBlazorResponse(Response userStoryResponse) => new(
        AccountId: userStoryResponse.AccountId,
        Email: userStoryResponse.Email!,
        Roles: userStoryResponse.Roles,
        ErrorMessage: userStoryResponse.ErrorMessage);
}
