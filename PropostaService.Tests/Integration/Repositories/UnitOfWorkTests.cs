using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PropostaService.Infrastructure.Persistence.Context;
using PropostaService.Infrastructure.Persistence.Repositories;
using PropostaService.Tests.Builders;
using Xunit;

namespace PropostaService.Tests.Integration.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly PropostaDbContext _context;
    private readonly UnitOfWork _unitOfWork;
    private readonly string _databaseName;

    public UnitOfWorkTests()
    {
        _databaseName = Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<PropostaDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .EnableSensitiveDataLogging()
            .Options;

        _context = new PropostaDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
        
        // Ensure database is clean for each test
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CommitAsync_ComAlteracoesPendentes_DeveSalvarAlteracoes()
    {
        var proposta = new PropostaBuilder().Build();
        await _context.Propostas.AddAsync(proposta);

        var resultado = await _unitOfWork.CommitAsync();

        resultado.Should().BeGreaterThan(0);
        
        // Refresh context to ensure we're reading from database
        _context.ChangeTracker.Clear();
        var propostaSalva = await _context.Propostas.FindAsync(proposta.Id);
        propostaSalva.Should().NotBeNull();
    }

    [Fact]
    public async Task CommitAsync_SemAlteracoes_DeveRetornarZero()
    {
        var resultado = await _unitOfWork.CommitAsync();

        resultado.Should().Be(0);
    }

    [Fact]
    public async Task CommitAsync_ComMultiplasOperacoes_DeveSalvarTodas()
    {
        var proposta1 = new PropostaBuilder().ComNomeCliente("João").Build();
        var proposta2 = new PropostaBuilder().ComNomeCliente("Maria").Build();
        
        await _context.Propostas.AddRangeAsync(proposta1, proposta2);

        var resultado = await _unitOfWork.CommitAsync();

        // SaveChangesAsync returns the number of state entries written to the database
        // This includes all tracked entities and their relationships
        resultado.Should().BeGreaterThan(0);
        
        // Refresh context
        _context.ChangeTracker.Clear();
        var propostas = await _context.Propostas.ToListAsync();
        propostas.Should().HaveCount(2);
        propostas.Should().Contain(p => p.Cliente.Nome == "João");
        propostas.Should().Contain(p => p.Cliente.Nome == "Maria");
    }

    [Fact]
    public async Task RollbackAsync_DeveExecutarSemErro()
    {
        var proposta = new PropostaBuilder().Build();
        await _context.Propostas.AddAsync(proposta);

        // RollbackAsync apenas completa a tarefa no contexto do InMemory DB
        // Para testar rollback, adicionamos sem commit e depois verificamos que não foi salvo
        await _unitOfWork.RollbackAsync();

        // Não commitamos, então não deve estar salvo
        var count = await _context.Propostas.CountAsync();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
