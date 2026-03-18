using Accounts.Register.UserStory;

namespace Accounts.Design.Register;

public class Then(FeatureDSL _dsl) {
    public async Task ShouldFailWith(string message) {
        var ex = await _dsl.ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldBe(message);
    }

    public async Task ShouldFailWith(string message, Action<FeatureDSL> assertion) {
        var ex = await _dsl.ShouldThrowAsync<InvalidOperationException>();
        ex.Message.ShouldBe(message);
        assertion(_dsl);
    }

    public async Task ShouldSucceed() => await _dsl.ShouldNotThrowAsync();

    public async Task ShouldSucceedWith(Action<Response> assertion) {
        var result = (Response)await _dsl.ExecuteAsync();
        assertion(result);
    }

    public async Task ShouldSucceedWith(Action<FeatureDSL, Response> assertion) {
        var result = (Response)await _dsl.ExecuteAsync();
        assertion(_dsl, result);
    }
}