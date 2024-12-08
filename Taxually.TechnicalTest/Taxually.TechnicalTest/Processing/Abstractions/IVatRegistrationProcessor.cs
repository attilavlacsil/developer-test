using Taxually.TechnicalTest.Models;

namespace Taxually.TechnicalTest.Processing.Abstractions;

public interface IVatRegistrationProcessor
{
    Task<VatRegistrationResult> ProcessAsync(VatRegistration vatRegistration, CancellationToken cancellationToken = default);
}
