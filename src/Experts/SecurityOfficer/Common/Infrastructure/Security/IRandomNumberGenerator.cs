
namespace Experts.SecurityOfficer.Common.Infrastructure.Security;
internal interface IRandomNumberGenerator {
    void Fill(Span<byte> data);
}

internal sealed class RandomNumberGenerator : IRandomNumberGenerator {
    public void Fill(Span<byte> data) => System.Security.Cryptography.RandomNumberGenerator.Fill(data);
}
