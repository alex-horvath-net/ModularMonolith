using Accounts.Core.Infrastructure;
using Core.Domain;
using Core.Infrastructure;

namespace Accounts.Register;

internal sealed class Save(IAccountRepository repsoitory, IClock clock) : WorkStep<Context>(clock) {
    protected override Task Run(Context context) =>
        repsoitory.CreateAccount(context.Account!, context.Token);
}
