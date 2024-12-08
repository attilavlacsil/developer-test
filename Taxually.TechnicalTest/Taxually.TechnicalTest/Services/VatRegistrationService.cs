using Microsoft.Extensions.Options;
using Taxually.TechnicalTest.Models;
using Taxually.TechnicalTest.Processing.Abstractions;
using Taxually.TechnicalTest.Processing.Options;

namespace Taxually.TechnicalTest.Services;

public sealed partial class VatRegistrationService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IOptionsMonitor<ProcessorOptions> processorOptions;
    private readonly ILogger<VatRegistrationService> logger;

    public VatRegistrationService(IServiceProvider serviceProvider, IOptionsMonitor<ProcessorOptions> processorOptions, ILogger<VatRegistrationService> logger)
    {
        this.serviceProvider = serviceProvider;
        this.processorOptions = processorOptions;
        this.logger = logger;
    }

    public async Task<VatRegistrationResult> RegisterAsync(VatRegistration vatRegistration, CancellationToken cancellationToken = default)
    {
        var country = vatRegistration.Country.Value;
        var processorName = processorOptions.Get(country).ProcessorName;
        await using var scope = serviceProvider.CreateAsyncScope();
        
        var processor = scope.ServiceProvider.GetKeyedService<IVatRegistrationProcessor>(processorName);
        if (processor is null)
        {
            LogNoProcessorForCountry(logger, country);
            return new VatRegistrationResult.Failure($"Invalid country: {country}");
        }

        return await processor.ProcessAsync(vatRegistration, cancellationToken);
    }

    [LoggerMessage(EventId = 1, Level = LogLevel.Error, Message = $"No processor registered for country {{{nameof(country)}}}")]
    private static partial void LogNoProcessorForCountry(ILogger logger, string country);
}
