using FluentAssertions;
using ContratacaoService.Domain.Exceptions;
using Xunit;

namespace ContratacaoService.Tests.Unit.Domain.Exceptions;

public class ContratoInvalidoExceptionTests
{
    [Fact]
    public void Constructor_ComMensagem_DeveCriarExcecao()
    {
        var mensagem = "Contrato inválido";

        var excecao = new ContratoInvalidoException(mensagem);

        excecao.Should().NotBeNull();
        excecao.Message.Should().Be(mensagem);
    }

    [Fact]
    public void ContratoInvalidoException_DeveHerdarDeDomainException()
    {
        var excecao = new ContratoInvalidoException("Teste");

        excecao.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void ContratoInvalidoException_DeveSerLancavel()
    {
        Action action = () => throw new ContratoInvalidoException("Contrato inválido");

        action.Should().Throw<ContratoInvalidoException>()
            .WithMessage("Contrato inválido");
    }
}
