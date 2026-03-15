using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class Fixtrure : Design.Fixtrure {
    internal Request Request { get; set; } = null!;
    internal Func<Request> RequestFactory { get; set; } = null!;
    internal Func<Task<Response>> SUT { get; set; } = null!;
    public Fixtrure() {
        SUT = () => {
            AccountantRepository = AccountRepositoryFactory();
            Hasher = HasherFactory();
            Clock = ClockFactory();

            Token = TokenFactory();
            Request = RequestFactory();

            var userStory = new UserStory(AccountantRepository, Hasher, Clock);
            return userStory.Register(Request, Token);
        };

        RequestFactory = () => new(
            Email: EmailFactory(),
            UserName: UserNameFactory(),
            Password: PasswordFactory(),
            Roles: RolesFactory());
    }
}

