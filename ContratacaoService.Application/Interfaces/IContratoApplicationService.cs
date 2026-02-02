using ContratacaoService.Application.DTOs;

namespace ContratacaoService.Application.Interfaces;

public interface IContratoApplicationService
{
    Task<ContratoDto> CriarContratoAsync(CriarContratoDto dto, CancellationToken cancellationToken = default);
    Task<ContratoDto?> ObterContratoPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ContratoDto>> ListarContratosAsync(CancellationToken cancellationToken = default);
}
