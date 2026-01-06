using BusinessExperts.Identity.CreateToken;

namespace BusinessExperts.Identity {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
