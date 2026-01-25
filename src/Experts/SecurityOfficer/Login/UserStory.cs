using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experts.SecurityOfficer.Login; 
public class UserStory {
    public Response Run(Request request) {
        return new Response(true);
    }

    public record Request();
    public record Response(
        bool IsUserStoryEnabled);
}
