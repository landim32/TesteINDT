namespace PropostaService.Application.DTOs;

public class CriarPropostaDto
{
    public string NomeCliente { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string TipoSeguro { get; set; } = string.Empty;
    public decimal ValorCobertura { get; set; }
    public decimal ValorPremio { get; set; }
}
