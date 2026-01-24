using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experts.Trader.Login;

public class UserStory {
    public Response Run(Request request) {
        return new Response(); 
    }

    public record Request();
    public record Response();
}
