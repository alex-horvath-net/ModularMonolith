using Experts.Experts.IdentityBusinessExpert.CreateToken;

namespace Experts.Experts.IdentityBusinessExpert {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
