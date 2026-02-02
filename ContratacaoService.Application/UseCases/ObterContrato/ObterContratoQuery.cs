using ContratacaoService.Application.DTOs;
using MediatR;

namespace ContratacaoService.Application.UseCases.ObterContrato;

public class ObterContratoQuery : IRequest<ContratoDto?>
{
    public Guid ContratoId { get; set; }

    public ObterContratoQuery(Guid contratoId)
    {
        ContratoId = contratoId;
    }
}
