namespace Experts.Experts.IdentityBusinessExpert.CreateToken;

public sealed record CreateTokenCommand(
    Guid JwtId,
    string Subject,
    DateTime IssuedAt);

