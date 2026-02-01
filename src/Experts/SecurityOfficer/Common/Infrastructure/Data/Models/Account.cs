namespace Experts.SecurityOfficer.Common.Infrastructure.Data.Models;

public class Account {
    public Guid Id { get; internal set; }
    public string UserName { get; internal set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsLocked { get; set; }
    public ISet<Role> Roles { get; set; } = new HashSet<Role>();
    public DateTime CreatedAtUtc { get; set; }
}
