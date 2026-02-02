using PropostaService.Domain.Enums;

namespace PropostaService.Api.ViewModels.Request;

public class AtualizarPropostaRequest
{
    public string? NomeCliente { get; set; }
    public decimal? ValorCobertura { get; set; }
    public decimal? ValorPremio { get; set; }
}
