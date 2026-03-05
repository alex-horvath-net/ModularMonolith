using System.Security.Cryptography;

namespace Core.Infrastructure.Random;

internal sealed class RandomGenerator : IRandom {
    public void Generate(Span<byte> data) => RandomNumberGenerator.Fill(data);
}

