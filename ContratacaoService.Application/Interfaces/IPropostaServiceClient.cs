using ContratacaoService.Application.DTOs;

namespace ContratacaoService.Application.Interfaces;

public interface IPropostaServiceClient
{
    Task<PropostaDto?> ObterPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<bool> VerificarPropostaAprovadaAsync(Guid propostaId, CancellationToken cancellationToken = default);
}
