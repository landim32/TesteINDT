using FluentAssertions;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.ValueObjects;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.ValueObjects;

public class CpfTests
{
    [Fact]
    public void Criar_ComCpfValido_DeveCriarCpf()
    {
        var cpf = "123.456.789-09";

        var result = Cpf.Criar(cpf);

        result.Should().NotBeNull();
        result.Valor.Should().Be("12345678909");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComCpfVazio_DeveLancarExcecao(string cpfInvalido)
    {
        var action = () => Cpf.Criar(cpfInvalido);

        action.Should().Throw<DomainException>()
            .WithMessage("CPF não pode ser vazio");
    }

    [Theory]
    [InlineData("123.456.789-00")]
    [InlineData("000.000.000-00")]
    [InlineData("111.111.111-11")]
    public void Criar_ComCpfInvalido_DeveLancarExcecao(string cpfInvalido)
    {
        var action = () => Cpf.Criar(cpfInvalido);

        action.Should().Throw<DomainException>()
            .WithMessage("CPF inválido");
    }

    [Fact]
    public void FormatarCpf_DeveRetornarCpfFormatado()
    {
        var cpf = Cpf.Criar("12345678909");

        var resultado = cpf.FormatarCpf();

        resultado.Should().Be("123.456.789-09");
    }

    [Fact]
    public void Equals_ComMesmoCpf_DeveRetornarTrue()
    {
        var cpf1 = Cpf.Criar("12345678909");
        var cpf2 = Cpf.Criar("123.456.789-09");

        var resultado = cpf1.Equals(cpf2);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void Equals_ComCpfDiferente_DeveRetornarFalse()
    {
        var cpf1 = Cpf.Criar("12345678909");
        var cpf2 = Cpf.Criar("98765432100");

        var resultado = cpf1.Equals(cpf2);

        resultado.Should().BeFalse();
    }
}
