using System.ComponentModel.DataAnnotations;

namespace ContratacaoService.Api.ViewModels.Request;

public class CriarContratoRequest
{
    [Required(ErrorMessage = "O ID da proposta é obrigatório")]
    public Guid PropostaId { get; set; }
}
