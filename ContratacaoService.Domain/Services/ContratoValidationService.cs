using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces.Repositories;
using ContratacaoService.Domain.Interfaces.Services;

namespace ContratacaoService.Domain.Services;

public class ContratoValidationService : IContratoValidationService
{
    private readonly IContratoRepository _contratoRepository;

    public ContratoValidationService(IContratoRepository contratoRepository)
    {
        _contratoRepository = contratoRepository;
    }

    public Task<bool> ValidarContratoAsync(Contrato contrato, CancellationToken cancellationToken = default)
    {
        if (contrato == null)
            return Task.FromResult(false);

        if (contrato.PropostaId == null || contrato.PropostaId.Value == Guid.Empty)
            return Task.FromResult(false);

        return Task.FromResult(true);
    }

    public async Task<bool> PodeContratarPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        var contratoExistente = await _contratoRepository.ExisteContratoParaPropostaAsync(propostaId, cancellationToken);
        return !contratoExistente;
    }
}
