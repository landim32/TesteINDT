using FluentAssertions;
using ContratacaoService.Domain.Specifications;
using ContratacaoService.Domain.Enums;
using ContratacaoService.Tests.Builders;
using Xunit;

namespace ContratacaoService.Tests.Unit.Domain.Specifications;

public class ContratoAtivoSpecificationTests
{
    private readonly ContratoAtivoSpecification _specification;

    public ContratoAtivoSpecificationTests()
    {
        _specification = new ContratoAtivoSpecification();
    }

    [Fact]
    public void IsSatisfiedBy_ComContratoAtivo_DeveRetornarTrue()
    {
        var contrato = new ContratoBuilder().Build();

        var resultado = _specification.IsSatisfiedBy(contrato);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_ComContratoCancelado_DeveRetornarFalse()
    {
        var contrato = new ContratoBuilder().BuildCancelado();

        var resultado = _specification.IsSatisfiedBy(contrato);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void IsSatisfiedBy_ComContratoSuspenso_DeveRetornarFalse()
    {
        var contrato = new ContratoBuilder().BuildSuspenso();

        var resultado = _specification.IsSatisfiedBy(contrato);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void ToExpression_DeveRetornarExpressaoValida()
    {
        var expression = _specification.ToExpression();

        expression.Should().NotBeNull();
    }

    [Fact]
    public void ToExpression_ComContratoAtivo_DeveRetornarTrue()
    {
        var contrato = new ContratoBuilder().Build();
        var expression = _specification.ToExpression();
        var compiledExpression = expression.Compile();

        var resultado = compiledExpression(contrato);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void ToExpression_ComContratoCancelado_DeveRetornarFalse()
    {
        var contrato = new ContratoBuilder().BuildCancelado();
        var expression = _specification.ToExpression();
        var compiledExpression = expression.Compile();

        var resultado = compiledExpression(contrato);

        resultado.Should().BeFalse();
    }
}
