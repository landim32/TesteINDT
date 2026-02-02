using FluentAssertions;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Services;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Services;

public class PropostaValidationServiceTests
{
    private readonly PropostaValidationService _validationService;

    public PropostaValidationServiceTests()
    {
        _validationService = new PropostaValidationService();
    }

    [Fact]
    public void ValidarProposta_ComPropostaValida_DeveRetornarTrue()
    {
        var proposta = new PropostaBuilder().Build();

        var resultado = _validationService.ValidarProposta(proposta);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void ValidarProposta_ComPropostaNula_DeveRetornarFalse()
    {
        var resultado = _validationService.ValidarProposta(null!);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void ObterErrosValidacao_ComPropostaValida_DeveRetornarListaVazia()
    {
        var proposta = new PropostaBuilder().Build();

        var erros = _validationService.ObterErrosValidacao(proposta);

        erros.Should().BeEmpty();
    }

    [Fact]
    public void ObterErrosValidacao_ComPropostaNula_DeveRetornarErro()
    {
        var erros = _validationService.ObterErrosValidacao(null!);

        erros.Should().Contain("Proposta não pode ser nula");
    }
}
