using System.Linq.Expressions;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.Domain.Specifications;

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
