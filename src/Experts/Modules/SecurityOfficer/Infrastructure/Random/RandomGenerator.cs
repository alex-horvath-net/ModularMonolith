using System.Security.Cryptography;

namespace Business.Modules.SecurityOfficer.Infrastructure.Random;

internal sealed class RandomGenerator : IRandom {
    public void Generate(Span<byte> data) => RandomNumberGenerator.Fill(data);
}

