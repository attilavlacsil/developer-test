namespace Taxually.TechnicalTest.Clients;

public class TaxuallyQueueClient
{
    public virtual Task EnqueueAsync<TPayload>(string queueName, TPayload payload, CancellationToken cancellationToken = default)
    {
        // Code to send to message queue removed for brevity
        return Task.CompletedTask;
    }
}
