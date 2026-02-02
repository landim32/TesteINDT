using FluentAssertions;
using PropostaService.Domain.Entities;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Entities;

public class ClienteTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarCliente()
    {
        var nome = "João da Silva";
        var cpf = "12345678909";

        var cliente = Cliente.Criar(nome, cpf);

        cliente.Should().NotBeNull();
        cliente.Nome.Should().Be("João da Silva");
        cliente.Cpf.Valor.Should().Be("12345678909");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComNomeVazio_DeveLancarExcecao(string nomeInvalido)
    {
        var action = () => Cliente.Criar(nomeInvalido, "12345678909");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Nome do cliente não pode ser vazio*");
    }

    [Theory]
    [InlineData("Jo")]
    [InlineData("A")]
    public void Criar_ComNomeMuitoCurto_DeveLancarExcecao(string nomeCurto)
    {
        var action = () => Cliente.Criar(nomeCurto, "12345678909");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Nome do cliente deve ter entre 3 e 200 caracteres*");
    }

    [Fact]
    public void Criar_ComNomeMuitoLongo_DeveLancarExcecao()
    {
        var nomeLongo = new string('A', 201);

        var action = () => Cliente.Criar(nomeLongo, "12345678909");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Nome do cliente deve ter entre 3 e 200 caracteres*");
    }

    [Fact]
    public void Criar_ComNomeComEspacos_DeveTrimarEspacos()
    {
        var nome = "  João da Silva  ";

        var cliente = Cliente.Criar(nome, "12345678909");

        cliente.Nome.Should().Be("João da Silva");
    }

    [Fact]
    public void AtualizarNome_ComNomeValido_DeveAtualizarNome()
    {
        var cliente = Cliente.Criar("João da Silva", "12345678909");

        cliente.AtualizarNome("Maria da Silva");

        cliente.Nome.Should().Be("Maria da Silva");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AtualizarNome_ComNomeVazio_DeveLancarExcecao(string nomeInvalido)
    {
        var cliente = Cliente.Criar("João da Silva", "12345678909");

        var action = () => cliente.AtualizarNome(nomeInvalido);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Nome do cliente não pode ser vazio*");
    }

    [Fact]
    public void AtualizarNome_ComNomeComEspacos_DeveTrimarEspacos()
    {
        var cliente = Cliente.Criar("João da Silva", "12345678909");

        cliente.AtualizarNome("  Maria da Silva  ");

        cliente.Nome.Should().Be("Maria da Silva");
    }
}
