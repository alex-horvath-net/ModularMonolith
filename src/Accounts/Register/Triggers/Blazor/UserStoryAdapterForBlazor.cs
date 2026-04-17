using Accounts.Register.UserStory;
using Core.Domain.Tasks;
namespace Accounts.Register.Triggers.Blazor;

internal sealed class UserStoryAdapterForBlazor(UserStory.UserStory userStory) : IRegister {

    public async Task<RegisterResponse> Run(RegisterRequest blazorRequest, CancellationToken token = default) {
        var request = blazorRequest.Map(ToUserStoryRequest);
        var context = new Context(request, token);

        await userStory.Execute(context);

        var response = context.ToResponse();
        var blazorResponse = ToBlazorResponse(response);
        return blazorResponse;
    }

    private Request ToUserStoryRequest(RegisterRequest blazorRequest) => new(
        Email: blazorRequest.Email,
        UserName: blazorRequest.UserName,
        Password: blazorRequest.Password,
        Roles: blazorRequest.Roles.ToArray(),
        CorrelationId: blazorRequest.CorrelationId,
        RequestId: blazorRequest.RunId!.Value);

    private RegisterResponse ToBlazorResponse(Response userStoryResponse) => new(
        AccountId: userStoryResponse.AccountId,
        Email: userStoryResponse.Email!,
        Roles: userStoryResponse.Roles,
        ErrorMessage: userStoryResponse.ErrorMessage);
}
