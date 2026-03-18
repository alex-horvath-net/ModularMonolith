using System.Globalization;
using Accounts.Core.Domain;

namespace Accounts.Design.Register;

public class Given(FeatureDSL _dsl) {
    public FeatureDSL RequestIsMissing() =>
        _dsl.Set<FeatureDSL>(x => x.RequestFactory = () => null!);

    public FeatureDSL EmailIsMissing() =>
        _dsl.Set<FeatureDSL>(x => x.EmailFactory = () => null!);

    public FeatureDSL EmailIsNotNormalized() =>
        _dsl.Set<FeatureDSL>(x => x.EmailFactory = () => " Test-Trader@Bank.Com  ");

    public FeatureDSL PasswordIsMissing() =>
        _dsl.Set<FeatureDSL>(x => x.PasswordFactory = () => null!);

    public FeatureDSL PasswordIsShorterThan(int trashold) =>
        _dsl.Set<FeatureDSL>(x => x.PasswordFactory = () => x.PasswordFactory()[..(trashold - 1)]);

    public FeatureDSL PasswordHasNoUpperCase() =>
        _dsl.Set<FeatureDSL>(x => x.PasswordFactory = () => x.PasswordFactory().ToLowerInvariant());

    public FeatureDSL PasswordHasNoLowerCase() =>
        _dsl.Set<FeatureDSL>(x => x.PasswordFactory = () => x.PasswordFactory().ToUpperInvariant());

    public FeatureDSL PasswordHasNoDigit() =>
        _dsl.Set<FeatureDSL>(x => x.PasswordFactory = () => new string(x.PasswordFactory().Where(c => !char.IsDigit(c)).ToArray()));

    public FeatureDSL PasswordHasNoSpecialCharacter() =>
        _dsl.Set<FeatureDSL>(x => x.PasswordFactory = () => new string(x.PasswordFactory().Where(c => char.IsLetterOrDigit(c)).ToArray()));

    public FeatureDSL UserNameIsMissing() =>
        _dsl.Set<FeatureDSL>(x => x.UserNameFactory = () => null!);

    public FeatureDSL UserNameIsNotNormalized() =>
        _dsl.Set<FeatureDSL>(x => x.UserNameFactory = () => " Test-Trader ");

    public FeatureDSL RolesIsMissing() =>
        _dsl.Set<FeatureDSL>(x => x.RolesFactory = () => null!);

    public FeatureDSL RolesAreNotNormailized() =>
        _dsl.Set<FeatureDSL>(x => x.RolesFactory = () => [null!, "", " "]);

    public FeatureDSL RolesAreNotNormalized() =>
        _dsl.Set<FeatureDSL>(x => x.RolesFactory = () => [null!, "", " ", "Trader", " TradeR "]);

    public FeatureDSL RolesContainUnregistered() =>
        _dsl.Set<FeatureDSL>(x => x.RolesFactory = () => ["Trader", "UnRegisteredRole"]);

    public FeatureDSL AccountAlreadyExistsWithSimilarEmail() =>
        _dsl.Set<FeatureDSL>(x => {
            var existingAccount = new Account(
                Guid.NewGuid(),
                x.EmailFactory(),
                x.UserNameFactory(),
                x.PasswordFactory(),
                x.RolesFactory().ToHashSet(StringComparer.OrdinalIgnoreCase),
                IsLocked: false,
                CreatedAtUtc: DateTime.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture));

            var mock = x.AccountRepositoryFactory();
            mock.FindAccountByEmail(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(existingAccount);
            x.AccountRepositoryFactory = () => mock;
        });
}