using FluentAssertions;
using PropostaService.Domain.Events;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Entities;

public class PropostaDomainEventsTests
{
    [Fact]
    public void Criar_DeveAdicionarEventoPropostaCriada()
    {
        var proposta = new PropostaBuilder().Build();

        proposta.DomainEvents.Should().HaveCount(1);
        proposta.DomainEvents.First().Should().BeOfType<PropostaCriadaEvent>();
    }

    [Fact]
    public void Criar_EventoPropostaCriada_DeveConterDadosCorretos()
    {
        var proposta = new PropostaBuilder()
            .ComNomeCliente("João da Silva")
            .ComCpf("12345678909")
            .Build();

        var evento = proposta.DomainEvents.First() as PropostaCriadaEvent;
        evento.Should().NotBeNull();
        evento!.PropostaId.Should().Be(proposta.Id);
        evento.NomeCliente.Should().Be("João da Silva");
        evento.Cpf.Should().Be("12345678909");
    }

    [Fact]
    public void Aprovar_DeveAdicionarEventoPropostaAprovada()
    {
        var proposta = new PropostaBuilder().Build();
        proposta.LimparEventos();

        proposta.Aprovar();

        proposta.DomainEvents.Should().HaveCount(1);
        proposta.DomainEvents.First().Should().BeOfType<PropostaAprovadaEvent>();
    }

    [Fact]
    public void Aprovar_EventoPropostaAprovada_DeveConterDadosCorretos()
    {
        var proposta = new PropostaBuilder()
            .ComNomeCliente("João da Silva")
            .Build();
        proposta.LimparEventos();

        proposta.Aprovar();

        var evento = proposta.DomainEvents.First() as PropostaAprovadaEvent;
        evento.Should().NotBeNull();
        evento!.PropostaId.Should().Be(proposta.Id);
        evento.NomeCliente.Should().Be("João da Silva");
    }

    [Fact]
    public void Rejeitar_DeveAdicionarEventoPropostaRejeitada()
    {
        var proposta = new PropostaBuilder().Build();
        proposta.LimparEventos();

        proposta.Rejeitar();

        proposta.DomainEvents.Should().HaveCount(1);
        proposta.DomainEvents.First().Should().BeOfType<PropostaRejeitadaEvent>();
    }

    [Fact]
    public void Rejeitar_EventoPropostaRejeitada_DeveConterDadosCorretos()
    {
        var proposta = new PropostaBuilder()
            .ComNomeCliente("João da Silva")
            .Build();
        proposta.LimparEventos();

        proposta.Rejeitar();

        var evento = proposta.DomainEvents.First() as PropostaRejeitadaEvent;
        evento.Should().NotBeNull();
        evento!.PropostaId.Should().Be(proposta.Id);
        evento.NomeCliente.Should().Be("João da Silva");
    }

    [Fact]
    public void LimparEventos_DeveLimparListaDeEventos()
    {
        var proposta = new PropostaBuilder().Build();

        proposta.LimparEventos();

        proposta.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void MultiplasOperacoes_DeveAcumularEventos()
    {
        var proposta = new PropostaBuilder().Build();

        proposta.Aprovar();

        proposta.DomainEvents.Should().HaveCount(2);
        proposta.DomainEvents.First().Should().BeOfType<PropostaCriadaEvent>();
        proposta.DomainEvents.Last().Should().BeOfType<PropostaAprovadaEvent>();
    }

    [Fact]
    public void DomainEvents_DeveSerReadOnly()
    {
        var proposta = new PropostaBuilder().Build();

        proposta.DomainEvents.Should().BeAssignableTo<IReadOnlyCollection<object>>();
    }
}
