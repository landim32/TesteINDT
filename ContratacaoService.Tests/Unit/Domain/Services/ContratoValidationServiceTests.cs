using FluentAssertions;
using Moq;
using ContratacaoService.Domain.Services;
using ContratacaoService.Domain.Interfaces.Repositories;
using ContratacaoService.Tests.Builders;
using Xunit;

namespace ContratacaoService.Tests.Unit.Domain.Services;

public class ContratoValidationServiceTests
{
    private readonly Mock<IContratoRepository> _contratoRepositoryMock;
    private readonly ContratoValidationService _service;

    public ContratoValidationServiceTests()
    {
        _contratoRepositoryMock = new Mock<IContratoRepository>();
        _service = new ContratoValidationService(_contratoRepositoryMock.Object);
    }

    [Fact]
    public async Task ValidarContratoAsync_ComContratoValido_DeveRetornarTrue()
    {
        var contrato = new ContratoBuilder().Build();

        var resultado = await _service.ValidarContratoAsync(contrato);

        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ValidarContratoAsync_ComContratoNulo_DeveRetornarFalse()
    {
        var resultado = await _service.ValidarContratoAsync(null!);

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task PodeContratarPropostaAsync_SemContratoExistente_DeveRetornarTrue()
    {
        var propostaId = Guid.NewGuid();
        _contratoRepositoryMock
            .Setup(x => x.ExisteContratoParaPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var resultado = await _service.PodeContratarPropostaAsync(propostaId);

        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task PodeContratarPropostaAsync_ComContratoExistente_DeveRetornarFalse()
    {
        var propostaId = Guid.NewGuid();
        _contratoRepositoryMock
            .Setup(x => x.ExisteContratoParaPropostaAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var resultado = await _service.PodeContratarPropostaAsync(propostaId);

        resultado.Should().BeFalse();
    }
}
