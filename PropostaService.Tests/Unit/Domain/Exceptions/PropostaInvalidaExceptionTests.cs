using FluentAssertions;
using PropostaService.Domain.Exceptions;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Exceptions;

public class PropostaInvalidaExceptionTests
{
    [Fact]
    public void Constructor_ComMensagem_DeveCriarExcecao()
    {
        var mensagem = "Proposta inválida";

        var excecao = new PropostaInvalidaException(mensagem);

        excecao.Should().NotBeNull();
        excecao.Message.Should().Be(mensagem);
    }

    [Fact]
    public void PropostaInvalidaException_DeveHerdarDeDomainException()
    {
        var excecao = new PropostaInvalidaException("Teste");

        excecao.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void PropostaInvalidaException_DeveSerLancavel()
    {
        Action action = () => throw new PropostaInvalidaException("Proposta inválida");

        action.Should().Throw<PropostaInvalidaException>()
            .WithMessage("Proposta inválida");
    }
}
