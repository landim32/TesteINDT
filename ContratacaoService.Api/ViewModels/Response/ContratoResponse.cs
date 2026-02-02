namespace ContratacaoService.Api.ViewModels.Response;

public class ContratoResponse
{
    public Guid Id { get; set; }
    public Guid PropostaId { get; set; }
    public DateTime DataContratacao { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
}
