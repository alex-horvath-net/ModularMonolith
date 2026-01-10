using BusinessExperts.IdentityBusinessExpert.CreateToken;

namespace BusinessExperts.IdentityBusinessExpert {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
