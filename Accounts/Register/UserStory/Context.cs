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

internal sealed record Context : ContextBase {
    public Context(Request? request, CancellationToken token) {
        Request = request;
        Token = token;
        CorellationId = request?.CorrelationId ?? Guid.Empty;
        RequestId = request?.RequestId;
    }

    public Request? Request { get; }
    public CancellationToken Token { get; }
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

