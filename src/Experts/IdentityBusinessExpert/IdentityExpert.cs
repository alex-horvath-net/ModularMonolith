using Business.Experts.IdentityBusinessExpert.CreateToken;

namespace Business.Experts.IdentityBusinessExpert {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
