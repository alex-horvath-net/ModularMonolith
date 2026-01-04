namespace Business.MemberApplicationUser.IdentityBusinessExpert.CreateToken;

public sealed record CreateTokenCommand(
    Guid JwtId,
    string Subject,
    DateTime IssuedAt);

