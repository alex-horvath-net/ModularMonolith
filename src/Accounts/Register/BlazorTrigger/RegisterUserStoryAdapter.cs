using Core.Domain.Tasks;
namespace Accounts.Register.BlazorTrigger;

internal sealed class RegisterUserStoryAdapter(UserStory userStory) : IRegisterAdapter {

    public async Task<RegisterBlazorResponse> Run(RegisterBlazorRequest blazorRequest, CancellationToken token = default) {
        var request = blazorRequest.Map(ToUserStoryRequest);
        var context = new Context(request, token);

        await userStory.Execute(context);

        var response = context.ToResponse();
        var blazorResponse = ToBlazorResponse(response);
        return blazorResponse;
    }

    private Request ToUserStoryRequest(RegisterBlazorRequest blazorRequest) => new(
        Email: blazorRequest.Email,
        UserName: blazorRequest.UserName,
        Password: blazorRequest.Password,
        Roles: blazorRequest.Roles.ToArray(),
        CorrelationId: blazorRequest.CorrelationId,
        RequestId: blazorRequest.RunId!.Value);

    private RegisterBlazorResponse ToBlazorResponse(Response userStoryResponse) => new(
        AccountId: userStoryResponse.AccountId,
        Email: userStoryResponse.Email!,
        Roles: userStoryResponse.Roles,
        ErrorMessage: userStoryResponse.ErrorMessage);
}
