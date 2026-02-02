using AutoMapper;
using FluentAssertions;
using Moq;
using PropostaService.Application.DTOs;
using PropostaService.Application.UseCases.ListarPropostas;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces.Repositories;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Unit.Application.UseCases;

public class ListarPropostasHandlerTests
{
    private readonly Mock<IPropostaRepository> _propostaRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ListarPropostasHandler _handler;

    public ListarPropostasHandlerTests()
    {
        _propostaRepositoryMock = new Mock<IPropostaRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ListarPropostasHandler(_propostaRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ComPropostasExistentes_DeveRetornarListaDePropostasDto()
    {
        var propostas = new List<Proposta>
        {
            new PropostaBuilder().Build(),
            new PropostaBuilder().ComNomeCliente("Maria da Silva").Build()
        };

        var propostasDto = new List<PropostaDto>
        {
            new PropostaDto(),
            new PropostaDto()
        };

        var query = new ListarPropostasQuery();

        _propostaRepositoryMock
            .Setup(x => x.ObterTodasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(propostas);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<PropostaDto>>(propostas))
            .Returns(propostasDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeSameAs(propostasDto);
        _propostaRepositoryMock.Verify(x => x.ObterTodasAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(x => x.Map<IEnumerable<PropostaDto>>(propostas), Times.Once);
    }

    [Fact]
    public async Task Handle_SemPropostas_DeveRetornarListaVazia()
    {
        var propostas = new List<Proposta>();
        var propostasDto = new List<PropostaDto>();
        var query = new ListarPropostasQuery();

        _propostaRepositoryMock
            .Setup(x => x.ObterTodasAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(propostas);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<PropostaDto>>(propostas))
            .Returns(propostasDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
