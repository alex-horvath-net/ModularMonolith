using Accounts.Core.Domain;

namespace Accounts.Register.UserStory;

internal enum RegistrationWorkStep {
    Validation,
    Normalization,
    PreventDuplication,
    CreateIdentity,
    SaveIdentity,
}

internal sealed record Context(Request Request, CancellationToken Token) {
    public Request? NormalizedRequest { get; set; }
    public Account? MachingAccount { get; set; }
    public Account? Account { get; set; }
    public List<RegistrationWorkStep> ExecutedWorkSteps { get; } = [];

    internal Response ToResponse() => new(
        ErrorMessage: null,
        Account!.Id,
        Account.Email,
        Account.UserName,
        Account.Roles);
}

