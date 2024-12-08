namespace Taxually.TechnicalTest.Dtos;

public sealed record VatRegistrationRequest
{
    public required string CompanyName { get; init; }
    public required string CompanyId { get; init; }
    public required string Country { get; init; }
}
