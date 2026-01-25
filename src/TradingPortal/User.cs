using Experts.SecurityOfficer.Shared.Domain;

namespace TradingPortal; 
public class User {

    public User() {
            
    }
    public void Set(ApplicationUser  user) {
        Application = user.Application;
        Identity = user.Identity;
        Roles = user.Roles;
    }

    public Application Application { get; set; }
    public Identity Identity { get; set; }
    public IReadOnlyList<string> Roles { get; set; }
}
