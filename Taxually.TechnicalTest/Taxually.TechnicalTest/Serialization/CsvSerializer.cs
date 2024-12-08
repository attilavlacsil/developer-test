using CsvHelper;
using System.Globalization;
using System.Text;
using Taxually.TechnicalTest.Serialization.Abstractions;

namespace Taxually.TechnicalTest.Serialization;

public sealed class CsvSerializer : ISerializer
{
    public async Task<Stream> SerializeToStreamAsync<T>(T item, CancellationToken cancellationToken = default)
    {
        var stream = new MemoryStream();

        await using (var streamWriter = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true))
        await using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true))
        {
            await csvWriter.WriteRecordsAsync([item], cancellationToken);
        }

        stream.Position = 0;

        return stream;
    }
}
