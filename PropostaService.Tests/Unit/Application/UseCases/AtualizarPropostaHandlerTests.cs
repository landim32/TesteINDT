using FluentAssertions;
using MediatR;
using Moq;
using PropostaService.Application.UseCases.AtualizarProposta;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Exceptions;
using PropostaService.Domain.Interfaces.Repositories;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Unit.Application.UseCases;

public class AtualizarPropostaHandlerTests
{
    private readonly Mock<IPropostaRepository> _propostaRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AtualizarPropostaHandler _handler;

    public AtualizarPropostaHandlerTests()
    {
        _propostaRepositoryMock = new Mock<IPropostaRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AtualizarPropostaHandler(_propostaRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ComPropostaExistente_DeveAtualizarProposta()
    {
        var propostaId = Guid.NewGuid();
        var proposta = new PropostaBuilder().Build();
        var command = new AtualizarPropostaCommand(
            propostaId,
            "Maria da Silva",
            60000,
            1200
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(MediatR.Unit.Value);
        _propostaRepositoryMock.Verify(x => x.AtualizarAsync(proposta, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ComPropostaInexistente_DeveLancarExcecao()
    {
        var propostaId = Guid.NewGuid();
        var command = new AtualizarPropostaCommand(
            propostaId,
            "Maria da Silva",
            null,
            null
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Proposta?)null);

        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<PropostaInvalidaException>()
            .WithMessage($"Proposta com ID {propostaId} não encontrada");
    }

    [Fact]
    public async Task Handle_ApenasComNomeCliente_DeveAtualizarApenasNome()
    {
        var propostaId = Guid.NewGuid();
        var proposta = new PropostaBuilder().Build();
        var valorCoberturaOriginal = proposta.Seguro.ValorCobertura.Valor;
        var valorPremioOriginal = proposta.Seguro.ValorPremio.Valor;
        
        var command = new AtualizarPropostaCommand(
            propostaId,
            "Maria da Silva",
            null,
            null
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        await _handler.Handle(command, CancellationToken.None);

        proposta.Cliente.Nome.Should().Be("Maria da Silva");
        proposta.Seguro.ValorCobertura.Valor.Should().Be(valorCoberturaOriginal);
        proposta.Seguro.ValorPremio.Valor.Should().Be(valorPremioOriginal);
    }

    [Fact]
    public async Task Handle_ApenasComValoresSeguro_DeveAtualizarApenasSeguro()
    {
        var propostaId = Guid.NewGuid();
        var proposta = new PropostaBuilder().Build();
        var nomeOriginal = proposta.Cliente.Nome;
        
        var command = new AtualizarPropostaCommand(
            propostaId,
            null,
            60000,
            1200
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        await _handler.Handle(command, CancellationToken.None);

        proposta.Cliente.Nome.Should().Be(nomeOriginal);
        proposta.Seguro.ValorCobertura.Valor.Should().Be(60000);
        proposta.Seguro.ValorPremio.Valor.Should().Be(1200);
    }

    [Fact]
    public async Task Handle_ComPropostaAprovada_DeveLancarExcecao()
    {
        var propostaId = Guid.NewGuid();
        var proposta = new PropostaBuilder().BuildAprovada();
        var command = new AtualizarPropostaCommand(
            propostaId,
            "Maria da Silva",
            null,
            null
        );

        _propostaRepositoryMock
            .Setup(x => x.ObterPorIdAsync(propostaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(proposta);

        var action = async () => await _handler.Handle(command, CancellationToken.None);

        await action.Should().ThrowAsync<PropostaInvalidaException>();
    }
}
