using ContratacaoService.Application.DTOs;
using MediatR;

namespace ContratacaoService.Application.UseCases.VerificarProposta;

public class VerificarPropostaQuery : IRequest<PropostaDto?>
{
    public Guid PropostaId { get; set; }

    public VerificarPropostaQuery(Guid propostaId)
    {
        PropostaId = propostaId;
    }
}
