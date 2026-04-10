using Accounts.Core.Domain;
using Core.Domain;

namespace Accounts.Register.UserStory;

public enum RegistrationWorkStep {
    Validation,
    Normalization,
    PreventDuplication,
    CreateIdentity,
    SaveIdentity,
}

internal sealed record Context(Request Request, CancellationToken Token) : ContextBase(Request.CorrelationId) {
    public Request? NormalizedRequest { get; set; }
    public Account? MachingAccount { get; set; }
    public Account? Account { get; set; }
    public List<RegistrationWorkStep> ExecutedBusinessWorkSteps { get; } = [];

    internal Response ToResponse() => new(
        ErrorMessage: null,
        Account!.Id,
        Account.Email,
        Account.UserName,
        Account.Roles);
}

