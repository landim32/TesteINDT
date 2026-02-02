using FluentAssertions;
using MediatR;
using Moq;
using PropostaService.Application.UseCases.AlterarStatusProposta;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Interfaces.Repositories;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Unit.Application.UseCases;

public class AlterarStatusPropostaHandlerTests
{
    private readonly Mock<IPropostaRepository> _propostaRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AlterarStatusPropostaHandler _handler;

    public AlterarStatusPropostaHandlerTests()
    {
        _propostaRepositoryMock = new Mock<IPropostaRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AlterarStatusPropostaHandler(_propostaRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComPropostaExistente_DeveAlterarStatus()
    {
        var propostaId = Guid.NewGuid();
        var proposta = new PropostaBuilder().Build();
        var command = new AlterarStatusPropostaCommand(
            propostaId,
            StatusProposta.Aprovada
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        proposta.Status.Should().Be(StatusProposta.Aprovada);
        _propostaRepositoryMock.Verify(x => x.AtualizarAsync(proposta, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComPropostaInexistente_DeveLancarExcecao()
    {
        var propostaId = Guid.NewGuid();
        var command = new AlterarStatusPropostaCommand(
            propostaId,
            StatusProposta.Aprovada
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Proposta?)null);

        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<PropostaInvalidaException>()
            .WithMessage($"Proposta com ID {propostaId} não encontrada");
    }

    [Fact]
    public async Task Handle_AlterarParaRejeitada_DeveRejeitarProposta()
    {
        var propostaId = Guid.NewGuid();
        var proposta = new PropostaBuilder().Build();
        var command = new AlterarStatusPropostaCommand(
            propostaId,
            StatusProposta.Rejeitada
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        await _handler.Handle(command, CancellationToken.None);

        proposta.Status.Should().Be(StatusProposta.Rejeitada);
    }

    [Fact]
    public async Task Handle_AlterarStatusPropostaAprovada_DeveAlterar()
    {
        var propostaId = Guid.NewGuid();
        var proposta = new PropostaBuilder().BuildAprovada();
        var command = new AlterarStatusPropostaCommand(
            propostaId,
            StatusProposta.Aprovada
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        await _handler.Handle(command, CancellationToken.None);

        proposta.Status.Should().Be(StatusProposta.Aprovada);
    }
}
