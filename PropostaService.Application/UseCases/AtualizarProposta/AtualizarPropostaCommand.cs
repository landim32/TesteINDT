using MediatR;

namespace PropostaService.Application.UseCases.AtualizarProposta;

public record AtualizarPropostaCommand(
    Guid PropostaId,
    string? NomeCliente,
    decimal? ValorCobertura,
    decimal? ValorPremio
) : IRequest<Unit>;
