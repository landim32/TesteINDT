using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.ValueObjects;

namespace ContratacaoService.Tests.Builders;

public class ContratoBuilder
{
    private Guid _propostaId = Guid.NewGuid();

    public ContratoBuilder ComPropostaId(Guid propostaId)
    {
        _propostaId = propostaId;
        return this;
    }

    public Contrato Build()
    {
        return new Contrato(new PropostaId(_propostaId));
    }

    public Contrato BuildCancelado()
    {
        var contrato = Build();
        contrato.Cancelar();
        return contrato;
    }

    public Contrato BuildSuspenso()
    {
        var contrato = Build();
        contrato.Suspender();
        return contrato;
    }

    public Contrato BuildExpirado()
    {
        var contrato = Build();
        contrato.Expirar();
        return contrato;
    }
}
