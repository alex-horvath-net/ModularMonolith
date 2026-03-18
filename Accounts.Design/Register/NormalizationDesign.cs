using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class NormalizationDesign : FeatureDSL {
    internal override UserStory Unit() => new(AccountantRepository, Hasher, Clock);
    internal override Task<Response> Call(UserStory userStory) => userStory.Register(Request, Token);
    internal override string WorkStep() => "Normalization";

    [Fact]
    public Task Email_Should_Be_Normalized() =>
        Given.EmailIsNotNormalized().
        When.Register().
        Then.ShouldSucceedWith(result => result.Email.ShouldBe("test-trader@bank.com"));

    [Fact]
    public Task UserName_Should_Be_Normalized() =>
        Given.UserNameIsNotNormalized().
        When.Register().
        Then.ShouldSucceedWith(result => result.UserName.ShouldBe("Test-Trader"));

    [Fact]
    public Task Roles_Should_Be_Normalized() =>
        Given.RolesAreNotNormalized().
        When.Register().
        Then.ShouldSucceedWith(result => result.Roles.ShouldBe(["Trader"]));
}
