namespace Experts.SecurityOfficer.Shared.Domain;

public record Identity(
    Guid VisitorId, 
    DateTime VisitStartedAt, 
    string AuthenticationId, 
    string UserName);
