namespace Business.Features.Accounts.Slices.Register;

public interface IUserStoryBlazorClient {
    Task<UserStoryBlazorClientResponse> Run(UserStoryBlazorClientRequest clientRequest, CancellationToken cancellationToken = default);
}

internal sealed class UserStoryBlazorClient(UserStory userStory) : IUserStoryBlazorClient {
    private readonly UserStory userStory = userStory ?? throw new ArgumentNullException(nameof(userStory));

    public async Task<UserStoryBlazorClientResponse> Run(UserStoryBlazorClientRequest clientRequest, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(clientRequest);

        var request = new UserStoryRequest(
            clientRequest.Email,
            clientRequest.UserName,
            clientRequest.Password,
            clientRequest.Roles.ToArray());

        var response = await userStory.Register(request, cancellationToken).ConfigureAwait(false);
        return new UserStoryBlazorClientResponse(response.AccountId, response.Email, response.Roles);
    }
}

public sealed class UserStoryBlazorClientRequest {
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

public sealed record UserStoryBlazorClientResponse(Guid AccountId, string Email, IReadOnlyCollection<string> Roles);
