namespace Core.Infrastructure.Random;

internal sealed class RandomNumberGenerator : IRandomNumberGenerator {
    public void New(Span<byte> data) => System.Security.Cryptography.RandomNumberGenerator.Fill(data);
}