
namespace Experts.SecurityOfficer.Shared.Infrastructure.Data.Models;

public class Account {
    public string Email { get; set; }
    public bool IsLocked { get;  set; }
    public string PasswordHash { get; set; }
}
