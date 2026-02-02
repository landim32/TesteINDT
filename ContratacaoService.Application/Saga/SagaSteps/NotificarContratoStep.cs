using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Application.Saga.SagaSteps;

public class NotificarContratoStep : ISagaStep<Contrato, bool>
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificarContratoStep> _logger;

    public NotificarContratoStep(
        IMediator mediator,
        ILogger<NotificarContratoStep> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<bool> ExecuteAsync(Contrato contrato, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Notificando criação do contrato {ContratoId}", contrato.Id);

        var evento = new ContratoCriadoEvent(
            contrato.Id,
            contrato.PropostaId.Value,
            contrato.DataContratacao);

        await _mediator.Publish(evento, cancellationToken);

        _logger.LogInformation("Notificação enviada para contrato {ContratoId}", contrato.Id);
        return true;
    }

    public Task CompensateAsync(Contrato contrato, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Compensação não necessária para NotificarContratoStep");
        return Task.CompletedTask;
    }
}
