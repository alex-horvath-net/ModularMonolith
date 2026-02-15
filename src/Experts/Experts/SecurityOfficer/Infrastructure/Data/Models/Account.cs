namespace Business.Experts.SecurityOfficer.Infrastructure.Data.Models;

public class Account {
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserNameNormalized { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmailNormalized { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime? PasswordChangedAtUtc { get; set; }
    public bool IsLocked { get; set; }
    public int FailedAccessCount { get; set; }
    public DateTime? LockoutEndUtc { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
    public ISet<Role> Roles { get; set; } = new HashSet<Role>();
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }
    public byte[] RowVersion { get; set; } = null!;
}

public class AccountMapper {
    public static Domain.Account? ToDomain(Account? dataAccount) => dataAccount == null ? null : new(
        dataAccount.Id,
            dataAccount.Email,
            dataAccount.UserName,
            dataAccount.PasswordHash,
            dataAccount.Roles.Select(RoleMapper.ToDomain).ToHashSet(),
            dataAccount.IsLocked,
            dataAccount.CreatedAtUtc);
}
