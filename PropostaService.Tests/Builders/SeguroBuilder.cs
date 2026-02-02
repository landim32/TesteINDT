using PropostaService.Domain.Entities;

namespace PropostaService.Tests.Builders;

public class SeguroBuilder
{
    private string _tipo = "Auto";
    private decimal _valorCobertura = 50000;
    private decimal _valorPremio = 1000;

    public SeguroBuilder ComTipo(string tipo)
    {
        _tipo = tipo;
        return this;
    }

    public SeguroBuilder ComValorCobertura(decimal valorCobertura)
    {
        _valorCobertura = valorCobertura;
        return this;
    }

    public SeguroBuilder ComValorPremio(decimal valorPremio)
    {
        _valorPremio = valorPremio;
        return this;
    }

    public Seguro Build()
    {
        return Seguro.Criar(_tipo, _valorCobertura, _valorPremio);
    }
}
