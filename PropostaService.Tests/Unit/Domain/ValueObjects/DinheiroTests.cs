using FluentAssertions;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.ValueObjects;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.ValueObjects;

public class DinheiroTests
{
    [Fact]
    public void Criar_ComValorValido_DeveCriarDinheiro()
    {
        var valor = 1000m;

        var dinheiro = Dinheiro.Criar(valor);

        dinheiro.Should().NotBeNull();
        dinheiro.Valor.Should().Be(1000m);
    }

    [Fact]
    public void Criar_ComValorZero_DeveCriarDinheiro()
    {
        var dinheiro = Dinheiro.Criar(0);

        dinheiro.Should().NotBeNull();
        dinheiro.Valor.Should().Be(0);
    }

    [Fact]
    public void Criar_ComValorNegativo_DeveLancarExcecao()
    {
        var action = () => Dinheiro.Criar(-100);

        action.Should().Throw<DomainException>()
            .WithMessage("Valor monetário não pode ser negativo");
    }

    [Fact]
    public void Zero_DeveRetornarDinheiroComValorZero()
    {
        var zero = Dinheiro.Zero;

        zero.Valor.Should().Be(0);
    }

    [Fact]
    public void Somar_ComValorValido_DeveSomarValores()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(500);

        var resultado = dinheiro1.Somar(dinheiro2);

        resultado.Valor.Should().Be(1500);
    }

    [Fact]
    public void Subtrair_ComValorValido_DeveSubtrairValores()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(500);

        var resultado = dinheiro1.Subtrair(dinheiro2);

        resultado.Valor.Should().Be(500);
    }

    [Fact]
    public void Subtrair_ResultadoNegativo_DeveLancarExcecao()
    {
        var dinheiro1 = Dinheiro.Criar(500);
        var dinheiro2 = Dinheiro.Criar(1000);

        var action = () => dinheiro1.Subtrair(dinheiro2);

        action.Should().Throw<DomainException>()
            .WithMessage("Resultado da subtração não pode ser negativo");
    }

    [Fact]
    public void Equals_ComMesmoValor_DeveRetornarTrue()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(1000);

        var resultado = dinheiro1.Equals(dinheiro2);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void Equals_ComValorDiferente_DeveRetornarFalse()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(500);

        var resultado = dinheiro1.Equals(dinheiro2);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void OperadorIgualdade_ComMesmoValor_DeveRetornarTrue()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(1000);

        var resultado = dinheiro1 == dinheiro2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void OperadorDesigualdade_ComValorDiferente_DeveRetornarTrue()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(500);

        var resultado = dinheiro1 != dinheiro2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void OperadorMaiorQue_ComValorMaior_DeveRetornarTrue()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(500);

        var resultado = dinheiro1 > dinheiro2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void OperadorMenorQue_ComValorMenor_DeveRetornarTrue()
    {
        var dinheiro1 = Dinheiro.Criar(500);
        var dinheiro2 = Dinheiro.Criar(1000);

        var resultado = dinheiro1 < dinheiro2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void OperadorMaiorOuIgual_ComValorIgual_DeveRetornarTrue()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(1000);

        var resultado = dinheiro1 >= dinheiro2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void OperadorMenorOuIgual_ComValorIgual_DeveRetornarTrue()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(1000);

        var resultado = dinheiro1 <= dinheiro2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveRetornarValorFormatado()
    {
        var dinheiro = Dinheiro.Criar(1000.50m);

        var resultado = dinheiro.ToString();

        resultado.Should().NotBeEmpty();
    }

    [Fact]
    public void GetHashCode_ComMesmoValor_DeveRetornarMesmoHashCode()
    {
        var dinheiro1 = Dinheiro.Criar(1000);
        var dinheiro2 = Dinheiro.Criar(1000);

        dinheiro1.GetHashCode().Should().Be(dinheiro2.GetHashCode());
    }
}
