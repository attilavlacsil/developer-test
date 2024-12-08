using System.Text;
using Taxually.TechnicalTest.Serialization.Abstractions;

namespace Taxually.TechnicalTest.Serialization;

public sealed class XmlSerializer : ISerializer
{
    public async Task<Stream> SerializeToStreamAsync<T>(T item, CancellationToken cancellationToken = default)
    {
        var stream = new MemoryStream();
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

        await using (var writer = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
        {
            serializer.Serialize(writer, item);
        }

        stream.Position = 0;

        return stream;

    }
}
