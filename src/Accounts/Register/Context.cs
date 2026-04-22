using Accounts.Core.Domain;
using Core.Domain;

namespace Accounts.Register;

internal sealed record Context(Request Request, CancellationToken Token) : ContextBase(Request.CorrelationId, Request.RequestId) {
    internal Request? NormalizedRequest { get; set; }
    internal Account? MachingAccount { get; set; }
    internal Account? Account { get; set; }
    internal Response ToResponse() => new(
        ErrorMessage: null,
        Account!.Id,
        Account.Email,
        Account.UserName,
        Account.Roles);
}

