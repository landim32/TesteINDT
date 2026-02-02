using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.Saga.SagaSteps;
using ContratacaoService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Application.Saga;

public class ContratacaoSaga
{
    private readonly VerificarPropostaStep _verificarPropostaStep;
    private readonly CriarContratoStep _criarContratoStep;
    private readonly NotificarContratoStep _notificarContratoStep;
    private readonly ILogger<ContratacaoSaga> _logger;

    public ContratacaoSaga(
        VerificarPropostaStep verificarPropostaStep,
        CriarContratoStep criarContratoStep,
        NotificarContratoStep notificarContratoStep,
        ILogger<ContratacaoSaga> logger)
    {
        _verificarPropostaStep = verificarPropostaStep;
        _criarContratoStep = criarContratoStep;
        _notificarContratoStep = notificarContratoStep;
        _logger = logger;
    }

    public async Task<Contrato> ExecutarAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando saga de contratação para proposta {PropostaId}", propostaId);

        PropostaDto? proposta = null;
        Contrato? contrato = null;

        try
        {
            proposta = await _verificarPropostaStep.ExecuteAsync(propostaId, cancellationToken);

            contrato = await _criarContratoStep.ExecuteAsync(proposta, cancellationToken);

            await _notificarContratoStep.ExecuteAsync(contrato, cancellationToken);

            _logger.LogInformation("Saga de contratação concluída com sucesso para proposta {PropostaId}", propostaId);
            return contrato;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na saga de contratação para proposta {PropostaId}. Iniciando compensação", propostaId);

            if (contrato != null && proposta != null)
            {
                await _criarContratoStep.CompensateAsync(proposta, cancellationToken);
            }

            throw;
        }
    }
}
