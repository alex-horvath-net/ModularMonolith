namespace Business.Experts.SecurityOfficer.Infrastructure;
internal interface IRandom {
    void Generate(Span<byte> data);
}
