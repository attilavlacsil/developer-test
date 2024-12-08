using Microsoft.Extensions.Options;
using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Processing.Abstractions;
using Taxually.TechnicalTest.Processing.Options;
using Taxually.TechnicalTest.Serialization.Abstractions;

namespace Taxually.TechnicalTest.Processing;

public sealed class CsvQueueVatRegistrationProcessor : QueueVatRegistrationProcessor
{
    private readonly ISerializer serializer;

    public CsvQueueVatRegistrationProcessor(
        TaxuallyQueueClient queueClient,
        IOptionsMonitor<ProcessorOptions> processorOptions,
        [FromKeyedServices(Serializers.CsvVatRegistration)] ISerializer serializer)
        : base(queueClient, processorOptions)
    {
        this.serializer = serializer;
    }

    protected override async Task<Stream> CreatePayloadStreamAsync(VatRegistration vatRegistration, CancellationToken cancellationToken = default)
    {
        var payload = new
        {
            CompanyName = vatRegistration.Company.Name,
            CompanyId = vatRegistration.Company.Id
        };

        return await serializer.SerializeToStreamAsync(payload, cancellationToken);
    }
}
