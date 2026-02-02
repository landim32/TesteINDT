using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces.Services;

namespace PropostaService.Domain.Services;

public class PropostaValidationService : IPropostaValidationService
{
    public bool ValidarProposta(Proposta proposta)
    {
        return !ObterErrosValidacao(proposta).Any();
    }

    public IEnumerable<string> ObterErrosValidacao(Proposta proposta)
    {
        var erros = new List<string>();

        if (proposta == null)
        {
            erros.Add("Proposta não pode ser nula");
            return erros;
        }

        if (proposta.Cliente == null)
            erros.Add("Cliente é obrigatório");

        if (proposta.Seguro == null)
            erros.Add("Seguro é obrigatório");

        if (proposta.DataCriacao > DateTime.UtcNow)
            erros.Add("Data de criação não pode ser no futuro");

        return erros;
    }
}
