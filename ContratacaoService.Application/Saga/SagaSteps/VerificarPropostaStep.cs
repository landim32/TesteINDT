using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Interfaces;
using ContratacaoService.Domain.Enums;
using ContratacaoService.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Application.Saga.SagaSteps;

public class VerificarPropostaStep : ISagaStep<Guid, PropostaDto>
{
    private readonly IPropostaServiceClient _propostaServiceClient;
    private readonly ILogger<VerificarPropostaStep> _logger;

    public VerificarPropostaStep(
        IPropostaServiceClient propostaServiceClient,
        ILogger<VerificarPropostaStep> logger)
    {
        _propostaServiceClient = propostaServiceClient;
        _logger = logger;
    }

    public async Task<PropostaDto> ExecuteAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Verificando proposta {PropostaId}", propostaId);

        var proposta = await _propostaServiceClient.ObterPropostaAsync(propostaId, cancellationToken);

        if (proposta == null)
        {
            _logger.LogWarning("Proposta {PropostaId} não encontrada", propostaId);
            throw new ContratoInvalidoException($"Proposta {propostaId} não encontrada");
        }

        if (proposta.Status != StatusProposta.Aprovada)
        {
            _logger.LogWarning("Proposta {PropostaId} não está aprovada. Status: {Status}", propostaId, proposta.Status);
            throw new PropostaNaoAprovadaException(propostaId);
        }

        _logger.LogInformation("Proposta {PropostaId} verificada com sucesso", propostaId);
        return proposta;
    }

    public Task CompensateAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Compensação não necessária para VerificarPropostaStep");
        return Task.CompletedTask;
    }
}
