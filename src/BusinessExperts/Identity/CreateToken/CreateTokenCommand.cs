namespace Experts.Identity.CreateToken;

public sealed record CreateTokenCommand(
    Guid JwtId,
    string Subject,
    DateTime IssuedAt);

