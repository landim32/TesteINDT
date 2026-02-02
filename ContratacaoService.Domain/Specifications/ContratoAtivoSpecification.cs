using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Enums;
using System.Linq.Expressions;

namespace ContratacaoService.Domain.Specifications;

public class ContratoAtivoSpecification : ISpecification<Contrato>
{
    public Expression<Func<Contrato, bool>> ToExpression()
    {
        return contrato => contrato.Status == StatusContrato.Ativo;
    }

    public bool IsSatisfiedBy(Contrato entity)
    {
        return entity.Status == StatusContrato.Ativo;
    }
}
