

namespace Experts.SecurityOfficer.Shared.Infrastructure.Data.Models;

public class Account {
    public Guid Id { get; internal set; }
    public string UserName { get; internal set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public bool IsLocked { get;  set; }
   
}
