namespace Business.Experts.SecurityOfficer.UserStories.CreateToken;

public sealed record CreateTokenCommand(
    Guid JwtId,
    string Subject,
    DateTime IssuedAt);

