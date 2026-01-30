namespace Tests.Shared;

[CollectionDefinition(Name)]
public class UserManualCollection : ICollectionFixture<PlaywrightFixture> {
    public const string Name = nameof(UserManualCollection);
}
