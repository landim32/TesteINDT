using FluentAssertions;
using PropostaService.Domain.Exceptions;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_ComMensagem_DeveCriarExcecao()
    {
        var mensagem = "Erro de domínio";

        var excecao = new DomainException(mensagem);

        excecao.Should().NotBeNull();
        excecao.Message.Should().Be(mensagem);
    }

    [Fact]
    public void Constructor_ComMensagemVazia_DeveCriarExcecao()
    {
        var excecao = new DomainException("");

        excecao.Should().NotBeNull();
        excecao.Message.Should().Be("");
    }

    [Fact]
    public void DomainException_DeveSerLancavel()
    {
        Action action = () => throw new DomainException("Teste");

        action.Should().Throw<DomainException>()
            .WithMessage("Teste");
    }
}
