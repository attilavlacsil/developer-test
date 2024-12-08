using Microsoft.Extensions.Options;
using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Processing.Options;

namespace Taxually.TechnicalTest.Processing.Abstractions;

public abstract class QueueVatRegistrationProcessor : IVatRegistrationProcessor
{
    private readonly TaxuallyQueueClient queueClient;
    private readonly IOptionsMonitor<ProcessorOptions> processorOptions;

    protected QueueVatRegistrationProcessor(TaxuallyQueueClient queueClient, IOptionsMonitor<ProcessorOptions> processorOptions)
    {
        this.queueClient = queueClient;
        this.processorOptions = processorOptions;
    }

    public async Task<VatRegistrationResult> ProcessAsync(VatRegistration vatRegistration, CancellationToken cancellationToken = default)
    {
        await using var stream = await CreatePayloadStreamAsync(vatRegistration, cancellationToken);

        var options = processorOptions.Get(vatRegistration.Country.Value);
        await queueClient.EnqueueAsync(options.Target, stream, cancellationToken);

        return new VatRegistrationResult.Success(Async: true);
    }

    protected abstract Task<Stream> CreatePayloadStreamAsync(VatRegistration vatRegistration, CancellationToken cancellationToken = default);
}
