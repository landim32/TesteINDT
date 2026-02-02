using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Domain.Interfaces.Services;

public interface IContratoValidationService
{
    Task<bool> ValidarContratoAsync(Contrato contrato, CancellationToken cancellationToken = default);
    Task<bool> PodeContratarPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default);
}
