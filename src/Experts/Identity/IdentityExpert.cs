using Experts.Identity.CreateToken;

namespace Experts.Identity {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
