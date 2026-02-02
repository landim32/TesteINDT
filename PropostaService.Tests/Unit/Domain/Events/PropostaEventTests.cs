using FluentAssertions;
using PropostaService.Domain.Events;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Events;

public class PropostaEventTests
{
    [Fact]
    public void PropostaCriadaEvent_ComDadosValidos_DeveCriarEvento()
    {
        var propostaId = Guid.NewGuid();
        var nomeCliente = "João da Silva";
        var cpf = "12345678909";

        var evento = new PropostaCriadaEvent(propostaId, nomeCliente, cpf);

        evento.Should().NotBeNull();
        evento.PropostaId.Should().Be(propostaId);
        evento.NomeCliente.Should().Be(nomeCliente);
        evento.Cpf.Should().Be(cpf);
    }

    [Fact]
    public void PropostaAprovadaEvent_ComDadosValidos_DeveCriarEvento()
    {
        var propostaId = Guid.NewGuid();
        var nomeCliente = "João da Silva";

        var evento = new PropostaAprovadaEvent(propostaId, nomeCliente);

        evento.Should().NotBeNull();
        evento.PropostaId.Should().Be(propostaId);
        evento.NomeCliente.Should().Be(nomeCliente);
    }

    [Fact]
    public void PropostaRejeitadaEvent_ComDadosValidos_DeveCriarEvento()
    {
        var propostaId = Guid.NewGuid();
        var nomeCliente = "João da Silva";

        var evento = new PropostaRejeitadaEvent(propostaId, nomeCliente);

        evento.Should().NotBeNull();
        evento.PropostaId.Should().Be(propostaId);
        evento.NomeCliente.Should().Be(nomeCliente);
    }
}
