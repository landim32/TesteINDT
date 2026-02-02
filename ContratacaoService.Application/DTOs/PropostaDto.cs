using ContratacaoService.Domain.Enums;

namespace ContratacaoService.Application.DTOs;

public class PropostaDto
{
    public Guid Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string TipoSeguro { get; set; } = string.Empty;
    public decimal ValorCobertura { get; set; }
    public decimal ValorPremio { get; set; }
    public DateTime DataCriacao { get; set; }
    public StatusProposta Status { get; set; }
}
