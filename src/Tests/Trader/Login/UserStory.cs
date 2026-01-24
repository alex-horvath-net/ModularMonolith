using FluentAssertions;

namespace Tests.Trader.Login; 
public class UserStory {
    [Fact]
    public void Login() {
        var userStory = new Experts.Trader.Login.UserStory();
        var request = new Experts.Trader.Login.UserStory.Request();
        var response = userStory.Run(request);
        response.Should().NotBeNull();
    }
}
  