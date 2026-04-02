using Accounts.Core.Infrastructure;
using Accounts.Register.WorkSteps;
using Core.Infrastructure;

namespace Accounts.Register.UserStory;

internal sealed class UserStory {
    internal IReadOnlyList<RegistrationWorkStep> ExecutedWorkSteps => context?.ExecutedWorkSteps ?? [];

    internal async Task<Response> Register(Request request, CancellationToken token) {
        context = new Context(request, token);

        context.ExecutedWorkSteps.Add(RegistrationWorkStep.Validation);
        validate.Run(context);

        context.ExecutedWorkSteps.Add(RegistrationWorkStep.Normalization);
        normalize.Run(context);

        context.ExecutedWorkSteps.Add(RegistrationWorkStep.PreventDuplication);
        await preventDuplication.Run(context);

        context.ExecutedWorkSteps.Add(RegistrationWorkStep.CreateIdentity);
        create.Run(context);

        context.ExecutedWorkSteps.Add(RegistrationWorkStep.SaveIdentity);
        await save.Run(context);

        //Activate email
        //Activate MFA

        return context.ToResponse();
    }

    internal UserStory(IAccountRepository repository, IHasher hasher, IClock clock) {
        validate = new Validate();
        normalize = new Normalize();
        preventDuplication = new PreventDuplication(repository);
        create = new Create(hasher, clock);
        save = new Save(repository);
    }

    private readonly Validate validate;
    private readonly Normalize normalize;
    private readonly PreventDuplication preventDuplication;
    private readonly Create create;
    private readonly Save save;
    private Context? context;
}
