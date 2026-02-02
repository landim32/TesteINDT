using FluentAssertions;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Enums;
using ContratacaoService.Domain.ValueObjects;
using ContratacaoService.Tests.Builders;
using Xunit;

namespace ContratacaoService.Tests.Unit.Domain.Entities;

public class ContratoTests
{
    [Fact]
    public void Criar_ComPropostaIdValido_DeveCriarContrato()
    {
        var propostaId = new PropostaId(Guid.NewGuid());

        var contrato = new Contrato(propostaId);

        contrato.Should().NotBeNull();
        contrato.Id.Should().NotBeEmpty();
        contrato.PropostaId.Should().Be(propostaId);
        contrato.Status.Should().Be(StatusContrato.Ativo);
        contrato.DataContratacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        contrato.DataCriacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        contrato.DataAtualizacao.Should().BeNull();
    }

    [Fact]
    public void Criar_ComPropostaIdNulo_DeveLancarExcecao()
    {
        Action action = () => new Contrato(null!);

        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Cancelar_ComContratoAtivo_DeveCancelarContrato()
    {
        var contrato = new ContratoBuilder().Build();

        contrato.Cancelar();

        contrato.Status.Should().Be(StatusContrato.Cancelado);
        contrato.DataAtualizacao.Should().NotBeNull();
        contrato.DataAtualizacao.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Cancelar_ComContratoJaCancelado_DeveLancarExcecao()
    {
        var contrato = new ContratoBuilder().BuildCancelado();

        Action action = () => contrato.Cancelar();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Contrato já está cancelado");
    }

    [Fact]
    public void Suspender_ComContratoAtivo_DeveSuspenderContrato()
    {
        var contrato = new ContratoBuilder().Build();

        contrato.Suspender();

        contrato.Status.Should().Be(StatusContrato.Suspenso);
        contrato.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Suspender_ComContratoNaoAtivo_DeveLancarExcecao()
    {
        var contrato = new ContratoBuilder().BuildCancelado();

        Action action = () => contrato.Suspender();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Apenas contratos ativos podem ser suspensos");
    }

    [Fact]
    public void Reativar_ComContratoSuspenso_DeveReativarContrato()
    {
        var contrato = new ContratoBuilder().BuildSuspenso();

        contrato.Reativar();

        contrato.Status.Should().Be(StatusContrato.Ativo);
        contrato.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Reativar_ComContratoNaoSuspenso_DeveLancarExcecao()
    {
        var contrato = new ContratoBuilder().Build();

        Action action = () => contrato.Reativar();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Apenas contratos suspensos podem ser reativados");
    }

    [Fact]
    public void Expirar_ComContratoAtivo_DeveExpirarContrato()
    {
        var contrato = new ContratoBuilder().Build();

        contrato.Expirar();

        contrato.Status.Should().Be(StatusContrato.Expirado);
        contrato.DataAtualizacao.Should().NotBeNull();
    }

    [Fact]
    public void Expirar_ComContratoCancelado_DeveLancarExcecao()
    {
        var contrato = new ContratoBuilder().BuildCancelado();

        Action action = () => contrato.Expirar();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Contrato cancelado não pode expirar");
    }

    [Fact]
    public void Expirar_ComContratoSuspenso_DeveExpirarContrato()
    {
        var contrato = new ContratoBuilder().BuildSuspenso();

        contrato.Expirar();

        contrato.Status.Should().Be(StatusContrato.Expirado);
    }
}
