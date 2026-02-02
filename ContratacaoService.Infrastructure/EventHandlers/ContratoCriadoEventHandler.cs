using ContratacaoService.Domain.Events;
using ContratacaoService.Infrastructure.ExternalServices.Messaging;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Infrastructure.EventHandlers;

public class ContratoCriadoEventHandler : INotificationHandler<ContratoCriadoEvent>
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ContratoCriadoEventHandler> _logger;

    public ContratoCriadoEventHandler(
        IMessagePublisher messagePublisher,
        IConfiguration configuration,
        ILogger<ContratoCriadoEventHandler> logger)
    {
        _messagePublisher = messagePublisher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task Handle(ContratoCriadoEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Publicando evento ContratoCriadoEvent para contrato {ContratoId} no RabbitMQ",
                notification.ContratoId);

            var exchange = _configuration["RabbitMQ:Exchanges:Contratos"] ?? "contratos.exchange";
            var routingKey = "contrato.criado";

            await _messagePublisher.PublishAsync(exchange, routingKey, notification, cancellationToken);

            _logger.LogInformation(
                "Evento ContratoCriadoEvent publicado com sucesso para contrato {ContratoId}",
                notification.ContratoId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Erro ao publicar evento ContratoCriadoEvent para contrato {ContratoId} no RabbitMQ",
                notification.ContratoId);
            throw;
        }
    }
}
