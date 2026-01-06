using Business.Identity.CreateToken;

namespace Business.Identity {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
