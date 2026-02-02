namespace PropostaService.Domain.Events;

public record PropostaCriadaEvent(Guid PropostaId, string NomeCliente, string Cpf);
