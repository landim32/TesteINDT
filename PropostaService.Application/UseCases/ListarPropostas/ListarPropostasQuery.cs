using MediatR;
using PropostaService.Application.DTOs;

namespace PropostaService.Application.UseCases.ListarPropostas;

public record ListarPropostasQuery : IRequest<IEnumerable<PropostaDto>>;
