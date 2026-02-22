using System.Security.Cryptography;
using Business.Infrastructure;

namespace Business.Infrastructure.Random;

internal sealed class RandomGenerator : IRandom {
    public void Generate(Span<byte> data) => RandomNumberGenerator.Fill(data);
}

