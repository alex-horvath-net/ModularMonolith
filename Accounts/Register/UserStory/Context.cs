using Accounts.Core.Domain;

namespace Accounts.Register.UserStory;

internal sealed record Context(Request Request, CancellationToken Token) {
    public Request? NormalizedRequest { get; set; }
    public Account? MachingAccount { get; set; }
    public Account? Account { get; set; }

    internal Response ToResponse() => new(
        ErrorMessage: null,
        Account!.Id,
        Account.Email,
        Account.UserName,
        Account.Roles);
}

