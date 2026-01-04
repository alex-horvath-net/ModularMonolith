using Business.ApplicationUsers.Member.Identity.CreateToken;

namespace Business.ApplicationUsers.Member.Identity {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
