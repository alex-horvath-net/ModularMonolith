namespace Experts.SecurityOfficer.Common.Infrastructure.Data.Models;

public class Role {
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class RoleMapper {
    public static string ToDomain(Role dataRole) =>
        dataRole.Name;
}