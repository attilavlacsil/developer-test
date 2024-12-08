namespace Taxually.TechnicalTest.Clients;

public class TaxuallyHttpClient
{
    public virtual Task PostAsync<TRequest>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        // Actual HTTP call removed for purposes of this exercise
        return Task.CompletedTask;
    }
}
