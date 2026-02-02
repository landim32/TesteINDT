using MediatR;
using PropostaService.Domain.Enums;

namespace PropostaService.Application.UseCases.AlterarStatusProposta;

public record AlterarStatusPropostaCommand(Guid PropostaId, StatusProposta NovoStatus) : IRequest<Unit>;
