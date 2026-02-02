using ContratacaoService.Domain.Entities;

namespace ContratacaoService.Domain.Interfaces.Repositories;

public interface IContratoRepository
{
    Task<Contrato?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Contrato?> ObterPorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Contrato>> ListarTodosAsync(CancellationToken cancellationToken = default);
    Task AdicionarAsync(Contrato contrato, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Contrato contrato, CancellationToken cancellationToken = default);
    Task<bool> ExisteContratoParaPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default);
}
