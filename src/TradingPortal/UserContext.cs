using Experts.SecurityOfficer.Shared.Domain;
using System.Threading.Tasks;

namespace TradingPortal;

public class UserContext {
    private readonly TaskCompletionSource<ApplicationUser> userReady = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public void Set(ApplicationUser user) {
        ArgumentNullException.ThrowIfNull(user);
        User = user;
        userReady.TrySetResult(user);
    }

    public Task<ApplicationUser> GetUserAsync() => userReady.Task;

    public ApplicationUser? User { get; private set; }
}