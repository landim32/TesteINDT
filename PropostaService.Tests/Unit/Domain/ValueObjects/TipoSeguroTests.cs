using FluentAssertions;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.ValueObjects;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.ValueObjects;

public class TipoSeguroTests
{
    [Fact]
    public void Criar_ComValorValido_DeveCriarTipoSeguro()
    {
        var valor = "Auto";

        var tipo = TipoSeguro.Criar(valor);

        tipo.Should().NotBeNull();
        tipo.Valor.Should().Be("Auto");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComValorVazio_DeveLancarExcecao(string valorInvalido)
    {
        var action = () => TipoSeguro.Criar(valorInvalido);

        action.Should().Throw<DomainException>()
            .WithMessage("Tipo de seguro não pode ser vazio");
    }

    [Theory]
    [InlineData("Ab")]
    [InlineData("A")]
    public void Criar_ComValorMuitoCurto_DeveLancarExcecao(string valorCurto)
    {
        var action = () => TipoSeguro.Criar(valorCurto);

        action.Should().Throw<DomainException>()
            .WithMessage("Tipo de seguro deve ter entre 3 e 50 caracteres");
    }

    [Fact]
    public void Criar_ComValorMuitoLongo_DeveLancarExcecao()
    {
        var valorLongo = new string('A', 51);

        var action = () => TipoSeguro.Criar(valorLongo);

        action.Should().Throw<DomainException>()
            .WithMessage("Tipo de seguro deve ter entre 3 e 50 caracteres");
    }

    [Fact]
    public void Criar_ComValorComEspacos_DeveTrimarEspacos()
    {
        var valor = "  Auto  ";

        var tipo = TipoSeguro.Criar(valor);

        tipo.Valor.Should().Be("Auto");
    }

    [Fact]
    public void Vida_DeveRetornarTipoVida()
    {
        var tipo = TipoSeguro.Vida;

        tipo.Valor.Should().Be("Vida");
    }

    [Fact]
    public void Auto_DeveRetornarTipoAuto()
    {
        var tipo = TipoSeguro.Auto;

        tipo.Valor.Should().Be("Auto");
    }

    [Fact]
    public void Residencial_DeveRetornarTipoResidencial()
    {
        var tipo = TipoSeguro.Residencial;

        tipo.Valor.Should().Be("Residencial");
    }

    [Fact]
    public void Saude_DeveRetornarTipoSaude()
    {
        var tipo = TipoSeguro.Saude;

        tipo.Valor.Should().Be("Saúde");
    }

    [Fact]
    public void Equals_ComMesmoValor_DeveRetornarTrue()
    {
        var tipo1 = TipoSeguro.Criar("Auto");
        var tipo2 = TipoSeguro.Criar("Auto");

        var resultado = tipo1.Equals(tipo2);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void Equals_ComMesmoValorCaseInsensitive_DeveRetornarTrue()
    {
        var tipo1 = TipoSeguro.Criar("Auto");
        var tipo2 = TipoSeguro.Criar("auto");

        var resultado = tipo1.Equals(tipo2);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void Equals_ComValorDiferente_DeveRetornarFalse()
    {
        var tipo1 = TipoSeguro.Criar("Auto");
        var tipo2 = TipoSeguro.Criar("Vida");

        var resultado = tipo1.Equals(tipo2);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void OperadorIgualdade_ComMesmoValor_DeveRetornarTrue()
    {
        var tipo1 = TipoSeguro.Criar("Auto");
        var tipo2 = TipoSeguro.Criar("Auto");

        var resultado = tipo1 == tipo2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void OperadorDesigualdade_ComValorDiferente_DeveRetornarTrue()
    {
        var tipo1 = TipoSeguro.Criar("Auto");
        var tipo2 = TipoSeguro.Criar("Vida");

        var resultado = tipo1 != tipo2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveRetornarValor()
    {
        var tipo = TipoSeguro.Criar("Auto");

        var resultado = tipo.ToString();

        resultado.Should().Be("Auto");
    }

    [Fact]
    public void GetHashCode_ComMesmoValor_DeveRetornarMesmoHashCode()
    {
        var tipo1 = TipoSeguro.Criar("Auto");
        var tipo2 = TipoSeguro.Criar("auto");

        tipo1.GetHashCode().Should().Be(tipo2.GetHashCode());
    }
}
