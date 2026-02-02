using FluentAssertions;
using ContratacaoService.Domain.ValueObjects;
using Xunit;

namespace ContratacaoService.Tests.Unit.Domain.ValueObjects;

public class PropostaIdTests
{
    [Fact]
    public void Criar_ComGuidValido_DeveCriarPropostaId()
    {
        var guid = Guid.NewGuid();

        var propostaId = new PropostaId(guid);

        propostaId.Should().NotBeNull();
        propostaId.Value.Should().Be(guid);
    }

    [Fact]
    public void Criar_ComGuidEmpty_DeveLancarExcecao()
    {
        Action action = () => new PropostaId(Guid.Empty);

        action.Should().Throw<ArgumentException>()
            .WithMessage("PropostaId não pode ser vazio*");
    }

    [Fact]
    public void ImplicitConversion_ParaGuid_DeveConverterCorretamente()
    {
        var guid = Guid.NewGuid();
        var propostaId = new PropostaId(guid);

        Guid resultado = propostaId;

        resultado.Should().Be(guid);
    }

    [Fact]
    public void ImplicitConversion_DeGuid_DeveConverterCorretamente()
    {
        var guid = Guid.NewGuid();

        PropostaId propostaId = guid;

        propostaId.Value.Should().Be(guid);
    }

    [Fact]
    public void Equals_ComMesmoValor_DeveRetornarTrue()
    {
        var guid = Guid.NewGuid();
        var propostaId1 = new PropostaId(guid);
        var propostaId2 = new PropostaId(guid);

        var resultado = propostaId1.Equals(propostaId2);

        resultado.Should().BeTrue();
    }

    [Fact]
    public void Equals_ComValorDiferente_DeveRetornarFalse()
    {
        var propostaId1 = new PropostaId(Guid.NewGuid());
        var propostaId2 = new PropostaId(Guid.NewGuid());

        var resultado = propostaId1.Equals(propostaId2);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void Equals_ComNull_DeveRetornarFalse()
    {
        var propostaId = new PropostaId(Guid.NewGuid());

        var resultado = propostaId.Equals(null);

        resultado.Should().BeFalse();
    }

    [Fact]
    public void OperadorIgualdade_ComMesmoValor_DeveRetornarTrue()
    {
        var guid = Guid.NewGuid();
        var propostaId1 = new PropostaId(guid);
        var propostaId2 = new PropostaId(guid);

        var resultado = propostaId1 == propostaId2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void OperadorDesigualdade_ComValorDiferente_DeveRetornarTrue()
    {
        var propostaId1 = new PropostaId(Guid.NewGuid());
        var propostaId2 = new PropostaId(Guid.NewGuid());

        var resultado = propostaId1 != propostaId2;

        resultado.Should().BeTrue();
    }

    [Fact]
    public void ToString_DeveRetornarGuidComoString()
    {
        var guid = Guid.NewGuid();
        var propostaId = new PropostaId(guid);

        var resultado = propostaId.ToString();

        resultado.Should().Be(guid.ToString());
    }

    [Fact]
    public void GetHashCode_ComMesmoValor_DeveRetornarMesmoHashCode()
    {
        var guid = Guid.NewGuid();
        var propostaId1 = new PropostaId(guid);
        var propostaId2 = new PropostaId(guid);

        propostaId1.GetHashCode().Should().Be(propostaId2.GetHashCode());
    }

    [Fact]
    public void OperadorIgualdade_AmbosNull_DeveRetornarTrue()
    {
        PropostaId? propostaId1 = null;
        PropostaId? propostaId2 = null;

        var resultado = propostaId1 == propostaId2;

        resultado.Should().BeTrue();
    }
}
