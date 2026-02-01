namespace Experts.SecurityOfficer.Register;

/// <summary>
/// Thin client adapter so UI layers can trigger the register business command without duplicating mapping logic.
/// </summary>
public sealed class UserStoryBlazorClient(UserStory userStory) {
    private readonly UserStory userStory = userStory ?? throw new ArgumentNullException(nameof(userStory));

    public async Task<ClientResponse> Run(ClientRequest clientRequest, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(clientRequest);

        var request = new UserStory.Request(
            clientRequest.Email,
            clientRequest.UserName,
            clientRequest.Password,
            clientRequest.Roles.ToArray());

        var response = await userStory.RegisterAsync(request, roles, cancellationToken).ConfigureAwait(false);
        return new ClientResponse(response.AccountId, response.Email, response.Roles);
    }

    public sealed class ClientRequest {
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public IList<string> Roles { get; } = [];

        public void ReplaceRoles(IEnumerable<string> roles) {
            ArgumentNullException.ThrowIfNull(roles);

            Roles.Clear();
            foreach (var role in roles) {
                if (!string.IsNullOrWhiteSpace(role)) {
                    Roles.Add(role);
                }
            }
        }
    }

    public sealed record ClientResponse(Guid AccountId, string Email, IReadOnlyCollection<string> Roles);
}
