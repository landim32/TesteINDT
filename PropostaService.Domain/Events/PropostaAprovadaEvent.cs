namespace PropostaService.Domain.Events;

public record PropostaAprovadaEvent(Guid PropostaId, string NomeCliente);
