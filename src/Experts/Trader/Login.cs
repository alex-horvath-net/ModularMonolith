namespace Experts.Trader; 
public class Login {
    public class UserStory {
        public Response Run(Request request) {
            return new Response(true);
        }

        public record Request();
        public record Response(
            bool IsUserStoryEnabled);

        public class BlazorClient {
            public Response Run(Request loginRequest) {
                var userStory = new UserStory();
                var userStoryRequest = new UserStory.Request();
                var userStoryResponse = userStory.Run(userStoryRequest);
                return new Response(userStoryResponse.IsUserStoryEnabled);
            }

            public record Request();
            public record Response(bool IsUserStoryEnabled);
        }

    }
}
