using ContratacaoService.Domain.Enums;

namespace ContratacaoService.Application.DTOs;

public class ContratoDto
{
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public DateTime DataContratacao { get; set; }
    public StatusContrato Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
