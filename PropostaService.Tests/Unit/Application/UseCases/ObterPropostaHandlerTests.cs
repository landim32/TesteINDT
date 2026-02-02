using AutoMapper;
using FluentAssertions;
using Moq;
using PropostaService.Application.DTOs;
using PropostaService.Application.UseCases.ObterProposta;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces.Repositories;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Unit.Application.UseCases;

public class ObterPropostaHandlerTests
{
    private readonly Mock<IPropostaRepository> _propostaRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ObterPropostaHandler _handler;

    public ObterPropostaHandlerTests()
    {
        _propostaRepositoryMock = new Mock<IPropostaRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ObterPropostaHandler(_propostaRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ComPropostaExistente_DeveRetornarPropostaDto()
    {
        var propostaId = Guid.NewGuid();
        var proposta = new PropostaBuilder().Build();
        var propostaDto = new PropostaDto();
        var query = new ObterPropostaQuery(propostaId);

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        _mapperMock
            .Setup(x => x.Map<PropostaDto>(proposta))
            .Returns(propostaDto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeSameAs(propostaDto);
        _propostaRepositoryMock.Verify(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(x => x.Map<PropostaDto>(proposta), Times.Once);
    }

    [Fact]
    public async Task Handle_ComPropostaInexistente_DeveRetornarNull()
    {
        var propostaId = Guid.NewGuid();
        var query = new ObterPropostaQuery(propostaId);

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Proposta?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
        _mapperMock.Verify(x => x.Map<PropostaDto>(It.IsAny<Proposta>()), Times.Never);
    }
}
