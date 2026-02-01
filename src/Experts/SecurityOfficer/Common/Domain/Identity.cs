namespace Experts.SecurityOfficer.Common.Domain;

public record Identity(
    Guid VisitorId,
    DateTime VisitStartedAt,
    string AuthenticationId,
    string UserName);
