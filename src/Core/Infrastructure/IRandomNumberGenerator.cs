namespace Core.Infrastructure;

public interface IRandomNumberGenerator {
    void New(Span<byte> data);
}
