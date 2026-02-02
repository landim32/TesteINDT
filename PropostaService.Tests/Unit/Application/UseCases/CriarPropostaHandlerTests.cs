using FluentAssertions;
using Moq;
using PropostaService.Application.UseCases.CriarProposta;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Interfaces.Repositories;
using Xunit;

namespace PropostaService.Tests.Unit.Application.UseCases;

public class CriarPropostaHandlerTests
{
    private readonly Mock<IPropostaRepository> _propostaRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CriarPropostaHandler _handler;

    public CriarPropostaHandlerTests()
    {
        _propostaRepositoryMock = new Mock<IPropostaRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CriarPropostaHandler(_propostaRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComDadosValidos_DeveCriarProposta()
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            "Auto",
            50000,
            1000
        );

        _unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
        _propostaRepositoryMock.Verify(x => x.AdicionarAsync(
            It.Is<Proposta>(p => p.Cliente.Nome == "João da Silva"), 
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarIdDaProposta()
    {
        var command = new CriarPropostaCommand(
            "João da Silva",
            "12345678909",
            "Auto",
            50000,
            1000
        );

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeEmpty();
    }
}
