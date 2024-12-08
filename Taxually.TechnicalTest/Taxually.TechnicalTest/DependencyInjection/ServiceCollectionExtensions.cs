using Taxually.TechnicalTest.Processing;
using Taxually.TechnicalTest.Processing.Abstractions;
using Taxually.TechnicalTest.Processing.Options;
using Taxually.TechnicalTest.Serialization;
using Taxually.TechnicalTest.Serialization.Abstractions;
using Taxually.TechnicalTest.Services;

namespace Taxually.TechnicalTest.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddVatRegistrationProcessing(this IServiceCollection services, IConfiguration configuration)
    {
        foreach (var section in configuration.GetChildren())
        {
            services.AddOptions<ProcessorOptions>(section.Key).Bind(section);
        }

        services.AddKeyedScoped<IVatRegistrationProcessor, HttpVatRegistrationProcessor>(Processors.HttpVatRegistration);
        services.AddKeyedScoped<IVatRegistrationProcessor, CsvQueueVatRegistrationProcessor>(Processors.CsvQueueVatRegistration);
        services.AddKeyedScoped<IVatRegistrationProcessor, XmlQueueVatRegistrationProcessor>(Processors.XmlQueueVatRegistration);

        services.AddKeyedSingleton<ISerializer, CsvSerializer>(Serializers.CsvVatRegistration);
        services.AddKeyedSingleton<ISerializer, XmlSerializer>(Serializers.XmlVatRegistration);

        services.AddSingleton<VatRegistrationService>();

        return services;
    }
}
