namespace Taxually.TechnicalTest.Processing.Options;

public sealed record ProcessorOptions
{
    public required string ProcessorName { get; init; }
    public required string Target { get; init; }
}
