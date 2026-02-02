using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Domain.Interfaces.Services;

public interface IPropostaValidationService
{
    Task<bool> ValidarPropostaAprovadaAsync(Proposta proposta, CancellationToken cancellationToken = default);
}
