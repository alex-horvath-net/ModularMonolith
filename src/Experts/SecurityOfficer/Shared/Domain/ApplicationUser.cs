using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Experts.SecurityOfficer.Shared.Domain;

public record ApplicationUser(Application Application, Identity Identity, IReadOnlyList<string> Roles);
