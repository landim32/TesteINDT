namespace PropostaService.Domain.Events;

public record PropostaRejeitadaEvent(Guid PropostaId, string NomeCliente);
