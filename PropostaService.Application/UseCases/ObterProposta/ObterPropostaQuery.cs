using MediatR;
using PropostaService.Application.DTOs;

namespace PropostaService.Application.UseCases.ObterProposta;

public record ObterPropostaQuery(Guid Id) : IRequest<PropostaDto?>;
