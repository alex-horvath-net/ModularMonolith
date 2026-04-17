namespace Accounts.UserManual;

[CollectionDefinition(Name)]
public class UserManualICollectionFixture : ICollectionFixture<PlaywrightFixture> {
    public const string Name = nameof(UserManualICollectionFixture);
}
