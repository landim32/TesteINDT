using MediatR;

namespace PropostaService.Application.UseCases.CriarProposta;

public record CriarPropostaCommand(
    string NomeCliente,
    string Cpf,
    string TipoSeguro,
    decimal ValorCobertura,
    decimal ValorPremio
) : IRequest<Guid>;
