using Business.Modules.SecurityOfficer.Domain;

namespace TradingPortal;

public class UserContext {
    public required ApplicationUser User { get; set; }
}