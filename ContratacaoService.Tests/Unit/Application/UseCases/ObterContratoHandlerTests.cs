using AutoMapper;
using FluentAssertions;
using Moq;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.UseCases.ObterContrato;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces.Repositories;
using ContratacaoService.Tests.Builders;
using Xunit;

namespace ContratacaoService.Tests.Unit.Application.UseCases;

public class ObterContratoHandlerTests
{
    private readonly Mock<IContratoRepository> _contratoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ObterContratoHandler _handler;

    public ObterContratoHandlerTests()
    {
        _contratoRepositoryMock = new Mock<IContratoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ObterContratoHandler(_contratoRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ComContratoExistente_DeveRetornarContratoDto()
    {
        var contratoId = Guid.NewGuid();
        var contrato = new ContratoBuilder().Build();
        var contratoDto = new ContratoDto();
        var query = new ObterContratoQuery(contratoId);

        _contratoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(contratoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(contrato);

        _mapperMock
            .Setup(x => x.Map<ContratoDto>(contrato))
            .Returns(contratoDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeSameAs(contratoDto);
        _contratoRepositoryMock.Verify(x => x.ObterPorIdAsync(contratoId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(x => x.Map<ContratoDto>(contrato), Times.Once);
    }

    [Fact]
    public async Task Handle_ComContratoInexistente_DeveRetornarNull()
    {
        var contratoId = Guid.NewGuid();
        var query = new ObterContratoQuery(contratoId);

        _contratoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(contratoId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contrato?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
        _mapperMock.Verify(x => x.Map<ContratoDto>(It.IsAny<Contrato>()), Times.Never);
    }
}
