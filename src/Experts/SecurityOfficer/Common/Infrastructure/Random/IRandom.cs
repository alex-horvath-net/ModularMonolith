namespace Experts.SecurityOfficer.Common.Infrastructure.Random;
internal interface IRandom {
    void Fill(Span<byte> data);
}
