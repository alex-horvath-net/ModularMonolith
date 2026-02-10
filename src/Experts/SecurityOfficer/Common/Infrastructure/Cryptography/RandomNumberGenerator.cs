namespace Experts.SecurityOfficer.Common.Infrastructure.Cryptography;

internal sealed class RandomNumberGenerator : IRandomNumberGenerator {
    public void Fill(Span<byte> data) => System.Security.Cryptography.RandomNumberGenerator.Fill(data);
}
