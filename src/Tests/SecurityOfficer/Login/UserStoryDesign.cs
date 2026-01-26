using Experts.SecurityOfficer.Login;
using FluentAssertions;

namespace Tests.SecurityOfficer.Login;  
public class UserStoryTests {
    [Fact]
    public void Login() {
        var userStory = new UserStory();
        var request = new UserStory.Request(
            Guid.Parse("10000000-0000-0000-0000-000000000001"),
            UserStory.AccountType.LocalAccount,
            new Dictionary<string, string>
            {
                ["Username"] = "TestUser",
                ["Password"] = "P@ssw0rd!"
            });
        var response = userStory.Run(request);
        response.Should().NotBeNull();
    }
}