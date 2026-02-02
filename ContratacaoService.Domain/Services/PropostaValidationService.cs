using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Enums;
using ContratacaoService.Domain.Interfaces.Services;

namespace ContratacaoService.Domain.Services;

public class PropostaValidationService : IPropostaValidationService
{
    public Task<bool> ValidarPropostaAprovadaAsync(Proposta proposta, CancellationToken cancellationToken = default)
    {
        if (proposta == null)
            return Task.FromResult(false);

        return Task.FromResult(proposta.Status == StatusProposta.Aprovada);
    }
}
