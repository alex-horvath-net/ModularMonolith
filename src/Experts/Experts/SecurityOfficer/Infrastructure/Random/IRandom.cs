namespace Business.Experts.SecurityOfficer.Infrastructure.Random;
internal interface IRandom {
    void Generate(Span<byte> data);
}
