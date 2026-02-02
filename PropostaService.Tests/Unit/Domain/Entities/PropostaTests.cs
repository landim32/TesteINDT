using FluentAssertions;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Exceptions;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Entities;

public class PropostaTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarProposta()
    {
        var proposta = Proposta.Criar(
            "João da Silva",
            "12345678909",
            "Auto",
            50000,
            1000
        );

        proposta.Should().NotBeNull();
        proposta.Id.Should().NotBeEmpty();
        proposta.Cliente.Nome.Should().Be("João da Silva");
        proposta.Cliente.Cpf.Valor.Should().Be("12345678909");
        proposta.Seguro.Tipo.Valor.Should().Be("Auto");
        proposta.Seguro.ValorCobertura.Valor.Should().Be(50000);
        proposta.Seguro.ValorPremio.Valor.Should().Be(1000);
        proposta.Status.Should().Be(StatusProposta.EmAnalise);
        proposta.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Aprovar_ComPropostaEmAnalise_DeveAprovarProposta()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);

        proposta.Aprovar();

        proposta.Status.Should().Be(StatusProposta.Aprovada);
    }

    [Fact]
    public void Aprovar_ComPropostaJaAprovada_DeveLancarExcecao()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);
        proposta.Aprovar();

        var action = () => proposta.Aprovar();

        action.Should().Throw<PropostaInvalidaException>()
            .WithMessage("Proposta já foi aprovada");
    }

    [Fact]
    public void Rejeitar_ComPropostaEmAnalise_DeveRejeitarProposta()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);

        proposta.Rejeitar();

        proposta.Status.Should().Be(StatusProposta.Rejeitada);
    }

    [Fact]
    public void Rejeitar_ComPropostaAprovada_DeveLancarExcecao()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);
        proposta.Aprovar();

        var action = () => proposta.Rejeitar();

        action.Should().Throw<PropostaInvalidaException>()
            .WithMessage("Proposta aprovada não pode ser rejeitada");
    }

    [Fact]
    public void AlterarStatus_ParaAprovada_DeveAprovarProposta()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);

        proposta.AlterarStatus(StatusProposta.Aprovada);

        proposta.Status.Should().Be(StatusProposta.Aprovada);
    }

    [Fact]
    public void AtualizarCliente_ComPropostaEmAnalise_DeveAtualizarCliente()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);

        proposta.AtualizarCliente("Maria da Silva");

        proposta.Cliente.Nome.Should().Be("Maria da Silva");
    }

    [Fact]
    public void AtualizarCliente_ComPropostaAprovada_DeveLancarExcecao()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);
        proposta.Aprovar();

        var action = () => proposta.AtualizarCliente("Maria da Silva");

        action.Should().Throw<PropostaInvalidaException>()
            .WithMessage("Não é possível atualizar proposta aprovada");
    }

    [Fact]
    public void AtualizarSeguro_ComPropostaEmAnalise_DeveAtualizarSeguro()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);

        proposta.AtualizarSeguro(60000, 1200);

        proposta.Seguro.ValorCobertura.Valor.Should().Be(60000);
        proposta.Seguro.ValorPremio.Valor.Should().Be(1200);
    }

    [Fact]
    public void AtualizarSeguro_ComPropostaRejeitada_DeveLancarExcecao()
    {
        var proposta = Proposta.Criar("João da Silva", "12345678909", "Auto", 50000, 1000);
        proposta.Rejeitar();

        var action = () => proposta.AtualizarSeguro(60000, 1200);

        action.Should().Throw<PropostaInvalidaException>()
            .WithMessage("Não é possível atualizar proposta rejeitada");
    }
}
