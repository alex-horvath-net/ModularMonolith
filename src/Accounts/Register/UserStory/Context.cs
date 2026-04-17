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

    internal Request? Request { get; }
    internal CancellationToken Token { get; }
    internal Request? NormalizedRequest { get; set; }
    internal Account? MachingAccount { get; set; }
    internal Account? Account { get; set; }
    internal List<RegistrationWorkStep> ExecutedBusinessWorkSteps { get; } = [];

    internal Response ToResponse() => new(
        ErrorMessage: null,
        Account!.Id,
        Account.Email,
        Account.UserName,
        Account.Roles);
}

