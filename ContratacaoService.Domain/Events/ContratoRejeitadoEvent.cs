using MediatR;

namespace ContratacaoService.Domain.Events;

public class ContratoRejeitadoEvent : INotification
{
    public Guid ContratoId { get; }
    public Guid PropostaId { get; }
    public string Motivo { get; }

    public ContratoRejeitadoEvent(Guid contratoId, Guid propostaId, string motivo)
    {
        ContratoId = contratoId;
        PropostaId = propostaId;
        Motivo = motivo;
    }
}
