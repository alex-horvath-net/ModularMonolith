using Features.Accounts.Domain;

namespace TradingPortal;

public class UserContext {
    public required ApplicationUser User { get; set; }
}