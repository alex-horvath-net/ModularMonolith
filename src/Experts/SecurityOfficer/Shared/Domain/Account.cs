namespace Experts.SecurityOfficer.Shared.Domain;

public class Account {
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsLocked { get; set; }
}
