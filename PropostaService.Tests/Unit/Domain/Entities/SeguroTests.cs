using FluentAssertions;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Exceptions;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Entities;

public class SeguroTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarSeguro()
    {
        var tipo = "Auto";
        var valorCobertura = 50000m;
        var valorPremio = 1000m;

        var seguro = Seguro.Criar(tipo, valorCobertura, valorPremio);

        seguro.Should().NotBeNull();
        seguro.Id.Should().NotBeEmpty();
        seguro.Tipo.Valor.Should().Be("Auto");
        seguro.ValorCobertura.Valor.Should().Be(50000m);
        seguro.ValorPremio.Valor.Should().Be(1000m);
    }

    [Fact]
    public void Criar_ComValorCoberturaZero_DeveLancarExcecao()
    {
        var action = () => Seguro.Criar("Auto", 0, 1000);

        action.Should().Throw<DomainException>()
            .WithMessage("Valor da cobertura deve ser maior que zero");
    }

    [Fact]
    public void Criar_ComValorCoberturaNegativo_DeveLancarExcecao()
    {
        var action = () => Seguro.Criar("Auto", -1000, 1000);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Criar_ComValorPremioZero_DeveLancarExcecao()
    {
        var action = () => Seguro.Criar("Auto", 50000, 0);

        action.Should().Throw<DomainException>()
            .WithMessage("Valor do prêmio deve ser maior que zero");
    }

    [Fact]
    public void Criar_ComValorPremioNegativo_DeveLancarExcecao()
    {
        var action = () => Seguro.Criar("Auto", 50000, -100);

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Criar_ComValorPremioMaiorQueCobertura_DeveLancarExcecao()
    {
        var action = () => Seguro.Criar("Auto", 1000, 2000);

        action.Should().Throw<DomainException>()
            .WithMessage("Valor do prêmio não pode ser maior ou igual ao valor da cobertura");
    }

    [Fact]
    public void Criar_ComValorPremioIgualCobertura_DeveLancarExcecao()
    {
        var action = () => Seguro.Criar("Auto", 1000, 1000);

        action.Should().Throw<DomainException>()
            .WithMessage("Valor do prêmio não pode ser maior ou igual ao valor da cobertura");
    }

    [Fact]
    public void AtualizarValores_ComDadosValidos_DeveAtualizarValores()
    {
        var seguro = Seguro.Criar("Auto", 50000, 1000);

        seguro.AtualizarValores(60000, 1200);

        seguro.ValorCobertura.Valor.Should().Be(60000);
        seguro.ValorPremio.Valor.Should().Be(1200);
    }

    [Fact]
    public void AtualizarValores_ComValorCoberturaZero_DeveLancarExcecao()
    {
        var seguro = Seguro.Criar("Auto", 50000, 1000);

        var action = () => seguro.AtualizarValores(0, 1000);

        action.Should().Throw<DomainException>()
            .WithMessage("Valor da cobertura deve ser maior que zero");
    }

    [Fact]
    public void AtualizarValores_ComValorPremioMaiorQueCobertura_DeveLancarExcecao()
    {
        var seguro = Seguro.Criar("Auto", 50000, 1000);

        var action = () => seguro.AtualizarValores(1000, 2000);

        action.Should().Throw<DomainException>()
            .WithMessage("Valor do prêmio não pode ser maior ou igual ao valor da cobertura");
    }
}
