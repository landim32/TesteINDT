using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.Domain.Interfaces.Repositories;

public interface IPropostaRepository
{
    Task<Proposta?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Proposta>> ObterTodasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Proposta>> ObterPorStatusAsync(StatusProposta status, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Proposta proposta, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Proposta proposta, CancellationToken cancellationToken = default);
}
