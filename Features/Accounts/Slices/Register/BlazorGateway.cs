using Common.Tasks;

namespace Features.Accounts.Slices.Register;

public class Blazor {
    public interface IGateway {
        Task<GatewayResponse> Run(GatewayRequest blazorRequest, CancellationToken token = default);
    }
    public sealed class GatewayRequest {
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
    public sealed record GatewayResponse(Guid AccountId, string Email, IReadOnlyCollection<string> Roles);

    internal sealed class Gateway(UserStory userStory) : IGateway {
        private readonly UserStory userStory = userStory ?? throw new ArgumentNullException(nameof(userStory));

        public Task<GatewayResponse> Run(GatewayRequest request, CancellationToken token = default) => userStory
            .Register(request.Map(ToUserStoryRequest), token)
            .Map(ToGatewayResponse);

        private UserStoryRequest ToUserStoryRequest(GatewayRequest gatewayRequest) => new(
            gatewayRequest.Email,
            gatewayRequest.UserName,
            gatewayRequest.Password,
            gatewayRequest.Roles.ToArray());

        private GatewayResponse ToGatewayResponse(UserStoryResponse userStoryResponse) => new(
            userStoryResponse.AccountId,
            userStoryResponse.Email!,
            userStoryResponse.Roles);
    }
}
