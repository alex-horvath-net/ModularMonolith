using Experts.IdentityBusinessExpert.CreateToken;

namespace Experts.IdentityBusinessExpert {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
