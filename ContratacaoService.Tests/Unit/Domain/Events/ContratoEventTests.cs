using FluentAssertions;
using ContratacaoService.Domain.Events;
using Xunit;

namespace ContratacaoService.Tests.Unit.Domain.Events;

public class ContratoEventTests
{
    [Fact]
    public void ContratoCriadoEvent_ComDadosValidos_DeveCriarEvento()
    {
        var contratoId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();
        var dataContratacao = DateTime.UtcNow;

        var evento = new ContratoCriadoEvent(contratoId, propostaId, dataContratacao);

        evento.Should().NotBeNull();
        evento.ContratoId.Should().Be(contratoId);
        evento.PropostaId.Should().Be(propostaId);
        evento.DataContratacao.Should().Be(dataContratacao);
    }

    [Fact]
    public void ContratoAprovadoEvent_ComDadosValidos_DeveCriarEvento()
    {
        var contratoId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();

        var evento = new ContratoAprovadoEvent(contratoId, propostaId);

        evento.Should().NotBeNull();
        evento.ContratoId.Should().Be(contratoId);
        evento.PropostaId.Should().Be(propostaId);
    }

    [Fact]
    public void ContratoRejeitadoEvent_ComDadosValidos_DeveCriarEvento()
    {
        var contratoId = Guid.NewGuid();
        var propostaId = Guid.NewGuid();
        var motivo = "Proposta não aprovada";

        var evento = new ContratoRejeitadoEvent(contratoId, propostaId, motivo);

        evento.Should().NotBeNull();
        evento.ContratoId.Should().Be(contratoId);
        evento.PropostaId.Should().Be(propostaId);
        evento.Motivo.Should().Be(motivo);
    }
}
