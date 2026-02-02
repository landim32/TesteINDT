using PropostaService.Domain.Enums;

namespace PropostaService.Application.DTOs;

public class AtualizarPropostaDto
{
    public Guid Id { get; set; }
    public string? NomeCliente { get; set; }
    public decimal? ValorCobertura { get; set; }
    public decimal? ValorPremio { get; set; }
    public StatusProposta? Status { get; set; }
}
