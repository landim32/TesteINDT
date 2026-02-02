using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Enums;
using PropostaService.Infrastructure.Persistence.Context;
using PropostaService.Infrastructure.Persistence.Repositories;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Integration.Repositories;

public class PropostaRepositoryTests : IDisposable
{
    private readonly PropostaDbContext _context;
    private readonly PropostaRepository _repository;

    public PropostaRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<PropostaDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PropostaDbContext(options);
        _repository = new PropostaRepository(_context);
    }

    [Fact]
    public async Task AdicionarAsync_ComPropostaValida_DeveAdicionarProposta()
    {
        var proposta = new PropostaBuilder().Build();

        await _repository.AdicionarAsync(proposta);
        await _context.SaveChangesAsync();

        var propostaRecuperada = await _context.Propostas.FindAsync(proposta.Id);
        propostaRecuperada.Should().NotBeNull();
        propostaRecuperada!.Id.Should().Be(proposta.Id);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarProposta()
    {
        var proposta = new PropostaBuilder().Build();
        await _context.Propostas.AddAsync(proposta);
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterPorIdAsync(proposta.Id);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(proposta.Id);
        resultado.Cliente.Nome.Should().Be(proposta.Cliente.Nome);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNull()
    {
        var idInexistente = Guid.NewGuid();

        var resultado = await _repository.ObterPorIdAsync(idInexistente);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodasAsync_ComMultiplasPropostas_DeveRetornarTodasOrdenadas()
    {
        var proposta1 = new PropostaBuilder().ComNomeCliente("João").Build();
        var proposta2 = new PropostaBuilder().ComNomeCliente("Maria").Build();
        await _context.Propostas.AddRangeAsync(proposta1, proposta2);
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterTodasAsync();

        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterTodasAsync_SemPropostas_DeveRetornarListaVazia()
    {
        var resultado = await _repository.ObterTodasAsync();

        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterPorStatusAsync_ComStatusEspecifico_DeveRetornarApenasPropostasComStatus()
    {
        var propostaAprovada = new PropostaBuilder().BuildAprovada();
        var propostaEmAnalise = new PropostaBuilder().Build();
        var propostaRejeitada = new PropostaBuilder().BuildRejeitada();

        await _context.Propostas.AddRangeAsync(propostaAprovada, propostaEmAnalise, propostaRejeitada);
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterPorStatusAsync(StatusProposta.Aprovada);

        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(1);
        resultado.First().Status.Should().Be(StatusProposta.Aprovada);
    }

    [Fact]
    public async Task ObterPorStatusAsync_SemPropostasComStatus_DeveRetornarListaVazia()
    {
        var proposta = new PropostaBuilder().Build();
        await _context.Propostas.AddAsync(proposta);
        await _context.SaveChangesAsync();

        var resultado = await _repository.ObterPorStatusAsync(StatusProposta.Aprovada);

        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task AtualizarAsync_ComPropostaExistente_DeveAtualizarProposta()
    {
        var proposta = new PropostaBuilder().Build();
        await _context.Propostas.AddAsync(proposta);
        await _context.SaveChangesAsync();

        proposta.AtualizarCliente("Maria da Silva");
        await _repository.AtualizarAsync(proposta);
        await _context.SaveChangesAsync();

        var propostaAtualizada = await _context.Propostas.FindAsync(proposta.Id);
        propostaAtualizada.Should().NotBeNull();
        propostaAtualizada!.Cliente.Nome.Should().Be("Maria da Silva");
    }

    [Fact]
    public async Task AtualizarAsync_ComStatusAlterado_DeveAtualizarStatus()
    {
        var proposta = new PropostaBuilder().Build();
        await _context.Propostas.AddAsync(proposta);
        await _context.SaveChangesAsync();

        proposta.Aprovar();
        await _repository.AtualizarAsync(proposta);
        await _context.SaveChangesAsync();

        var propostaAtualizada = await _context.Propostas.FindAsync(proposta.Id);
        propostaAtualizada.Should().NotBeNull();
        propostaAtualizada!.Status.Should().Be(StatusProposta.Aprovada);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
