using ContratacaoService.Application.DTOs;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces.Repositories;
using ContratacaoService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace ContratacaoService.Application.Saga.SagaSteps;

public class CriarContratoStep : ISagaStep<PropostaDto, Contrato>
{
    private readonly IContratoRepository _contratoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CriarContratoStep> _logger;

    public CriarContratoStep(
        IContratoRepository contratoRepository,
        IUnitOfWork unitOfWork,
        ILogger<CriarContratoStep> logger)
    {
        _contratoRepository = contratoRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Contrato> ExecuteAsync(PropostaDto proposta, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Criando contrato para proposta {PropostaId}", proposta.Id);

        var contrato = new Contrato(new PropostaId(proposta.Id));
        await _contratoRepository.AdicionarAsync(contrato, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation("Contrato {ContratoId} criado com sucesso para proposta {PropostaId}", contrato.Id, proposta.Id);
        return contrato;
    }

    public async Task CompensateAsync(PropostaDto proposta, CancellationToken cancellationToken = default)
    {
        _logger.LogWarning("Compensando criação de contrato para proposta {PropostaId}", proposta.Id);

        var contrato = await _contratoRepository.ObterPorPropostaIdAsync(proposta.Id, cancellationToken);
        if (contrato != null)
        {
            contrato.Cancelar();
            await _contratoRepository.AtualizarAsync(contrato, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            _logger.LogInformation("Contrato {ContratoId} cancelado com sucesso", contrato.Id);
        }
    }
}
