using Microsoft.Extensions.Options;
using Taxually.TechnicalTest.Clients;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Processing.Abstractions;
using Taxually.TechnicalTest.Processing.Options;

namespace Taxually.TechnicalTest.Processing;

public sealed class HttpVatRegistrationProcessor : IVatRegistrationProcessor
{
    private readonly TaxuallyHttpClient httpClient;
    private readonly IOptionsMonitor<ProcessorOptions> processorOptions;

    public HttpVatRegistrationProcessor(TaxuallyHttpClient httpClient, IOptionsMonitor<ProcessorOptions> processorOptions)
    {
        this.httpClient = httpClient;
        this.processorOptions = processorOptions;
    }

    public async Task<VatRegistrationResult> ProcessAsync(VatRegistration vatRegistration, CancellationToken cancellationToken = default)
    {
        var options = processorOptions.Get(vatRegistration.Country.Value);
        var payload = new HttpVatRegistration(vatRegistration.Company.Name, vatRegistration.Company.Id, vatRegistration.Country.Value);

        await httpClient.PostAsync(options.Target, payload, cancellationToken);

        return new VatRegistrationResult.Success(Async: false);
    }

    private sealed record HttpVatRegistration(string CompanyName, string CompanyId, string Country);
}
