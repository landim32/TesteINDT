using MediatR;

namespace ContratacaoService.Domain.Events;

public class ContratoCriadoEvent : INotification
{
    public Guid ContratoId { get; }
    public Guid PropostaId { get; }
    public DateTime DataContratacao { get; }

    public ContratoCriadoEvent(Guid contratoId, Guid propostaId, DateTime dataContratacao)
    {
        ContratoId = contratoId;
        PropostaId = propostaId;
        DataContratacao = dataContratacao;
    }
}
