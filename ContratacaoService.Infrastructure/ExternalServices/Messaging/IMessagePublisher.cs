namespace ContratacaoService.Infrastructure.ExternalServices.Messaging;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string exchange, string routingKey, T message, CancellationToken cancellationToken = default);
}
