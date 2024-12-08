using Microsoft.Extensions.Options;
using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Processing.Abstractions;
using Taxually.TechnicalTest.Processing.Options;
using Taxually.TechnicalTest.Serialization.Abstractions;

namespace Taxually.TechnicalTest.Processing;

public sealed class XmlQueueVatRegistrationProcessor : QueueVatRegistrationProcessor
{
    private readonly ISerializer serializer;

    public XmlQueueVatRegistrationProcessor(
        TaxuallyQueueClient queueClient,
        IOptionsMonitor<ProcessorOptions> processorOptions,
        [FromKeyedServices(Serializers.XmlVatRegistration)] ISerializer serializer)
        : base(queueClient, processorOptions)
    {
        this.serializer = serializer;
    }

    protected override async Task<Stream> CreatePayloadStreamAsync(VatRegistration vatRegistration, CancellationToken cancellationToken = default)
    {
        var payload = new XmlVatRegistration
        {
            CompanyName = vatRegistration.Company.Name,
            CompanyId = vatRegistration.Company.Id,
            Country = vatRegistration.Country.Value
        };

        return await serializer.SerializeToStreamAsync(payload, cancellationToken);
    }

    /// <remarks>This should be <see langword="private"/>, but XML serializer expects a <see langword="public"/> class.</remarks>
    public sealed class XmlVatRegistration
    {
        public required string CompanyName { get; init; }
        public required string CompanyId { get; init; }
        public required string Country { get; init; }
    }
}
