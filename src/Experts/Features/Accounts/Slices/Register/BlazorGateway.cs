using Common.Tasks;

namespace Core.Features.Accounts.Slices.Register;

public interface IBlazorGateway {
    Task<BlazorGatewayResponse> Run(BlazorGatewayRequest blazorRequest, CancellationToken token = default);
}

internal sealed class BlazorGateway(UserStory userStory) : IBlazorGateway {
    private readonly UserStory userStory = userStory ?? throw new ArgumentNullException(nameof(userStory));

    public Task<BlazorGatewayResponse> Run(BlazorGatewayRequest request, CancellationToken token = default) => userStory
        .Register(request.Map(ToUserStoryRequest), token)
        .Map(ToBlazorResponse);

    private UserStoryRequest ToUserStoryRequest(BlazorGatewayRequest blazorRequest) => new(
        blazorRequest.Email,
        blazorRequest.UserName,
        blazorRequest.Password,
        blazorRequest.Roles.ToArray());

    private BlazorGatewayResponse ToBlazorResponse(UserStoryResponse response) => new(
        response.AccountId,
        response.Email,
        response.Roles);
}

public sealed class BlazorGatewayRequest {
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public IList<string> Roles { get; } = [];

    public void ReplaceRoles(IEnumerable<string> roles) {
        ArgumentNullException.ThrowIfNull(roles);

        Roles.Clear();
        foreach (var role in roles)
            if (!string.IsNullOrWhiteSpace(role))
                Roles.Add(role);
    }
}

public sealed record BlazorGatewayResponse(Guid AccountId, string Email, IReadOnlyCollection<string> Roles);
