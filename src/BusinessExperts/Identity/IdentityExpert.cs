using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessExperts.Identity.CreateToken;

namespace BusinessExperts.Identity {
    public record IdentityExpert(
        CreateTokenCommandHandler CreateToken) {
    }
}
