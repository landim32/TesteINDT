using PropostaService.Domain.Enums;

namespace PropostaService.Api.ViewModels.Request;

public class AlterarStatusPropostaRequest
{
    public StatusProposta Status { get; set; }
}
