using FluentAssertions;

namespace Tests.SecurityOfficer.Login;  
public class UserStory {
    [Fact]
    public void Login() {
        var userStory = new Experts.SecurityOfficer.Login.UserStory();
        var request = new Experts.SecurityOfficer.Login.UserStory.Request();
        var response = userStory.Run(request);
        response.Should().NotBeNull();
    }
}
  