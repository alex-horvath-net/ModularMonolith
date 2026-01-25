using Experts.SecurityOfficer.Shared.Domain;

namespace TradingPortal;

public class UserContext {
    public void Set(ApplicationUser user) {
        User = user;
    }

    public ApplicationUser User { get; private set; }
}