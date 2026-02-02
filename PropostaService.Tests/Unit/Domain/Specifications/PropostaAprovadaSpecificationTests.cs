using FluentAssertions;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Specifications;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Unit.Domain.Specifications;

public class PropostaAprovadaSpecificationTests
{
    private readonly PropostaAprovadaSpecification _specification;

    public PropostaAprovadaSpecificationTests()
    {
        _specification = new PropostaAprovadaSpecification();
    }

    [Fact]
    public void IsSatisfiedBy_ComPropostaAprovada_DeveRetornarTrue()
    {
        var proposta = new PropostaBuilder().BuildAprovada();

        var resultado = _specification.IsSatisfiedBy(proposta);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_ComPropostaEmAnalise_DeveRetornarFalse()
    {
        var proposta = new PropostaBuilder().Build();

        var resultado = _specification.IsSatisfiedBy(proposta);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_ComPropostaRejeitada_DeveRetornarFalse()
    {
        var proposta = new PropostaBuilder().BuildRejeitada();

        var resultado = _specification.IsSatisfiedBy(proposta);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void ToExpression_DeveRetornarExpressaoValida()
    {
        var expression = _specification.ToExpression();

        expression.Should().NotBeNull();
    }

    [Fact]
    public void ToExpression_ComPropostaAprovada_DeveRetornarTrue()
    {
        var proposta = new PropostaBuilder().BuildAprovada();
        var expression = _specification.ToExpression();
        var compiledExpression = expression.Compile();

        var resultado = compiledExpression(proposta);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void ToExpression_ComPropostaEmAnalise_DeveRetornarFalse()
    {
        var proposta = new PropostaBuilder().Build();
        var expression = _specification.ToExpression();
        var compiledExpression = expression.Compile();

        var resultado = compiledExpression(proposta);

        resultado.Should().BeFalse();
    }
}
