using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Enums;
using System.Linq.Expressions;

namespace ContratacaoService.Domain.Specifications;

public class PropostaAprovadaSpecification : ISpecification<Proposta>
{
    public Expression<Func<Proposta, bool>> ToExpression()
    {
        return proposta => proposta.Status == StatusProposta.Aprovada;
    }

    public bool IsSatisfiedBy(Proposta entity)
    {
        return entity.Status == StatusProposta.Aprovada;
    }
}
