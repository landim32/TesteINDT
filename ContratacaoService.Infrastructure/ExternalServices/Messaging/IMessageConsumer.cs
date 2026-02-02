namespace ContratacaoService.Infrastructure.ExternalServices.Messaging;

public interface IMessageConsumer
{
    Task<T?> ConsumeAsync<T>(string queue, CancellationToken cancellationToken = default);
    void StartConsuming<T>(string queue, Func<T, Task> handler);
    void StopConsuming();
}
