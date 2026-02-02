using PropostaService.Domain.Entities;

namespace PropostaService.Domain.Interfaces.Services;

public interface IPropostaValidationService
{
    bool ValidarProposta(Proposta proposta);
    IEnumerable<string> ObterErrosValidacao(Proposta proposta);
}
