using ContratacaoService.Application.DTOs;
using MediatR;

namespace ContratacaoService.Application.UseCases.CriarContrato;

public class CriarContratoCommand : IRequest<ContratoDto>
{
    public Guid PropostaId { get; set; }

    public CriarContratoCommand(Guid propostaId)
    {
        PropostaId = propostaId;
    }
}
