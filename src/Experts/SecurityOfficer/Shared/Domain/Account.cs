namespace Experts.SecurityOfficer.Shared.Domain;

public class Account {
    public bool IsLocked { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}
