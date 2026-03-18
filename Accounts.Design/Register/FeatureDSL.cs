using Accounts.Core.Infrastructure;
using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public abstract class FeatureDSL : ModuleDSL {
    internal Request Request { get; set; } = null!;
    internal Func<Request> RequestFactory { get; set; }
    internal IAccountRepository AccountRepository => AccountantRepository;
    internal CancellationToken CurrentToken => Token;
    internal Request CurrentRequest => Request;

    protected FeatureDSL() {
        RequestFactory = () => new Request(
            Email: EmailFactory(),
            UserName: UserNameFactory(),
            Password: PasswordFactory(),
            Roles: RolesFactory());
    }

    protected override void Build() {
        AccountantRepository = AccountRepositoryFactory();
        Hasher = HasherFactory();
        Clock = ClockFactory();

        Token = TokenFactory();
        Request = RequestFactory();
    }

    internal abstract UserStory Unit();
    internal abstract Task<Response> Call(UserStory userStory);

    internal abstract string WorkStep();

    protected override async Task<object> ExecuteUnit() {
        var unit = Unit();
        return await Call(unit);
    }

    public Given Given => new(this);
    public When When => new(this);
    public Then Then => new(this);
}