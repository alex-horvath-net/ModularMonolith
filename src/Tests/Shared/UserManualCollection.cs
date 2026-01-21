using Xunit;

namespace Tests.Shared;

[CollectionDefinition(Name)]
public class UserManualCollection : ICollectionFixture<TradingPortalPlaywrigh>
{
    public const string Name = nameof(UserManualCollection);
}
