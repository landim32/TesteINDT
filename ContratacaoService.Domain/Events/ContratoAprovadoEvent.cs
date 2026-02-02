using MediatR;

namespace ContratacaoService.Domain.Events;

public class ContratoAprovadoEvent : INotification
{
    public Guid ContratoId { get; }
    public Guid PropostaId { get; }

    public ContratoAprovadoEvent(Guid contratoId, Guid propostaId)
    {
        ContratoId = contratoId;
        PropostaId = propostaId;
    }
}
