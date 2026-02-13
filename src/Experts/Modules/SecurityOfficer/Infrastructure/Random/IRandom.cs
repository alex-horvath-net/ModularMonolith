namespace Business.Modules.SecurityOfficer.Infrastructure.Random;
internal interface IRandom {
    void Generate(Span<byte> data);
}
