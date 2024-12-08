namespace Taxually.TechnicalTest.Models;

public abstract record VatRegistrationResult
{
    public sealed record Success(bool Async) : VatRegistrationResult;
    public sealed record Failure(string Error) : VatRegistrationResult;
}
