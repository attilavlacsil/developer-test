namespace Taxually.TechnicalTest.Serialization.Abstractions;

public interface ISerializer
{
    Task<Stream> SerializeToStreamAsync<T>(T item, CancellationToken cancellationToken = default);
}
