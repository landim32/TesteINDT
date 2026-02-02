using PropostaService.Domain.ValueObjects;
using PropostaService.Domain.Exceptions;

namespace PropostaService.Domain.Entities;

public class Seguro
{
    public Guid Id { get; private set; }
    public TipoSeguro Tipo { get; private set; }
    public Dinheiro ValorCobertura { get; private set; }
    public Dinheiro ValorPremio { get; private set; }

    protected Seguro()
    {
        Id = Guid.Empty;
        Tipo = null!;
        ValorCobertura = null!;
        ValorPremio = null!;
    }

    private Seguro(TipoSeguro tipo, Dinheiro valorCobertura, Dinheiro valorPremio)
    {
        Id = Guid.NewGuid();
        Tipo = tipo;
        ValorCobertura = valorCobertura;
        ValorPremio = valorPremio;
    }

    public static Seguro Criar(string tipo, decimal valorCobertura, decimal valorPremio)
    {
        var tipoSeguro = TipoSeguro.Criar(tipo);
        var cobertura = Dinheiro.Criar(valorCobertura);
        var premio = Dinheiro.Criar(valorPremio);

        if (cobertura <= Dinheiro.Zero)
            throw new DomainException("Valor da cobertura deve ser maior que zero");

        if (premio <= Dinheiro.Zero)
            throw new DomainException("Valor do prêmio deve ser maior que zero");

        if (premio >= cobertura)
            throw new DomainException("Valor do prêmio não pode ser maior ou igual ao valor da cobertura");

        return new Seguro(tipoSeguro, cobertura, premio);
    }

    public void AtualizarValores(decimal valorCobertura, decimal valorPremio)
    {
        var cobertura = Dinheiro.Criar(valorCobertura);
        var premio = Dinheiro.Criar(valorPremio);

        if (cobertura <= Dinheiro.Zero)
            throw new DomainException("Valor da cobertura deve ser maior que zero");

        if (premio <= Dinheiro.Zero)
            throw new DomainException("Valor do prêmio deve ser maior que zero");

        if (premio >= cobertura)
            throw new DomainException("Valor do prêmio não pode ser maior ou igual ao valor da cobertura");

        ValorCobertura = cobertura;
        ValorPremio = premio;
    }
}
