namespace Experts.SecurityOfficer.CreateToken;

public sealed record CreateTokenCommand(
    Guid JwtId,
    string Subject,
    DateTime IssuedAt);

