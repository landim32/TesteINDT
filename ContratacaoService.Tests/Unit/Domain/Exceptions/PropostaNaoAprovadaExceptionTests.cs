using FluentAssertions;
using ContratacaoService.Domain.Exceptions;
using Xunit;

namespace ContratacaoService.Tests.Unit.Domain.Exceptions;

public class PropostaNaoAprovadaExceptionTests
{
    [Fact]
    public void Constructor_ComMensagem_DeveCriarExcecao()
    {
        var mensagem = "Proposta não aprovada";

        var excecao = new PropostaNaoAprovadaException(mensagem);

        excecao.Should().NotBeNull();
        excecao.Message.Should().Be(mensagem);
    }

    [Fact]
    public void PropostaNaoAprovadaException_DeveHerdarDeDomainException()
    {
        var excecao = new PropostaNaoAprovadaException("Teste");

        excecao.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void PropostaNaoAprovadaException_DeveSerLancavel()
    {
        Action action = () => throw new PropostaNaoAprovadaException("Proposta não aprovada");

        action.Should().Throw<PropostaNaoAprovadaException>()
            .WithMessage("Proposta não aprovada");
    }
}
