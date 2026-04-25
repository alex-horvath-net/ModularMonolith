using Accounts.Core.Domain;

namespace Accounts.Login;

internal sealed record Context(UserStoryRequest Request, UserStoryResponse Response, CancellationToken Token) {
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? Password { get; set; }
    internal Account? Account { get; set; }
}
