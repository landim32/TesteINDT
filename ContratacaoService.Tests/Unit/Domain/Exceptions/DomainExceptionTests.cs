using FluentAssertions;
using ContratacaoService.Domain.Exceptions;
using Xunit;

namespace ContratacaoService.Tests.Unit.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void ContratoInvalidoException_ComMensagem_DeveCriarExcecao()
    {
        var mensagem = "Erro de domínio";

        var excecao = new ContratoInvalidoException(mensagem);

        excecao.Should().NotBeNull();
        excecao.Message.Should().Be(mensagem);
        excecao.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void PropostaNaoAprovadaException_ComMensagem_DeveCriarExcecao()
    {
        var mensagem = "Proposta não aprovada";

        var excecao = new PropostaNaoAprovadaException(mensagem);

        excecao.Should().NotBeNull();
        excecao.Message.Should().Be(mensagem);
        excecao.Should().BeAssignableTo<DomainException>();
    }

    [Fact]
    public void DomainException_DeveSerClasseAbstrata()
    {
        var type = typeof(DomainException);

        type.Should().BeAbstract();
        type.Should().BeDerivedFrom<Exception>();
    }
}
