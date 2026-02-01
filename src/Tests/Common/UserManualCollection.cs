namespace Tests.Common;

[CollectionDefinition(Name)]
public class UserManualICollectionFixture : ICollectionFixture<PlaywrightFixture> {
    public const string Name = nameof(UserManualICollectionFixture);
}
