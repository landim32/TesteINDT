using AutoMapper;
using FluentAssertions;
using Moq;
using ContratacaoService.Application.DTOs;
using ContratacaoService.Application.UseCases.ListarContratos;
using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces.Repositories;
using ContratacaoService.Tests.Builders;
using Xunit;

namespace ContratacaoService.Tests.Unit.Application.UseCases;

public class ListarContratosHandlerTests
{
    private readonly Mock<IContratoRepository> _contratoRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ListarContratosHandler _handler;

    public ListarContratosHandlerTests()
    {
        _contratoRepositoryMock = new Mock<IContratoRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ListarContratosHandler(_contratoRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ComContratosExistentes_DeveRetornarListaDeContratosDto()
    {
        var contratos = new List<Contrato>
        {
            new ContratoBuilder().Build(),
            new ContratoBuilder().Build()
        };

        var contratosDto = new List<ContratoDto>
        {
            new ContratoDto(),
            new ContratoDto()
        };

        var query = new ListarContratosQuery();

        _contratoRepositoryMock
            .Setup(x => x.ListarTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(contratos);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<ContratoDto>>(contratos))
            .Returns(contratosDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeSameAs(contratosDto);
        _contratoRepositoryMock.Verify(x => x.ListarTodosAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(x => x.Map<IEnumerable<ContratoDto>>(contratos), Times.Once);
    }

    [Fact]
    public async Task Handle_SemContratos_DeveRetornarListaVazia()
    {
        var contratos = new List<Contrato>();
        var contratosDto = new List<ContratoDto>();
        var query = new ListarContratosQuery();

        _contratoRepositoryMock
            .Setup(x => x.ListarTodosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(contratos);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<ContratoDto>>(contratos))
            .Returns(contratosDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
