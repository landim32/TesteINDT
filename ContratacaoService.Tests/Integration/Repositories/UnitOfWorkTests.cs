using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ContratacaoService.Infrastructure.Persistence.Context;
using ContratacaoService.Infrastructure.Persistence.Repositories;
using ContratacaoService.Tests.Builders;
using Xunit;

namespace ContratacaoService.Tests.Integration.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly ContratacaoDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<ContratacaoDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        _context = new ContratacaoDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
        
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CommitAsync_ComAlteracoesPendentes_DeveSalvarAlteracoes()
    {
        var contrato = new ContratoBuilder().Build();
        await _context.Contratos.AddAsync(contrato);

        var resultado = await _unitOfWork.CommitAsync();

        resultado.Should().BeGreaterThan(0);
        
        _context.ChangeTracker.Clear();
        var contratoSalvo = await _context.Contratos.FindAsync(contrato.Id);
        contratoSalvo.Should().NotBeNull();
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
        var contrato1 = new ContratoBuilder().Build();
        var contrato2 = new ContratoBuilder().Build();
        
        await _context.Contratos.AddRangeAsync(contrato1, contrato2);

        var resultado = await _unitOfWork.CommitAsync();

        resultado.Should().BeGreaterThan(0);
        
        _context.ChangeTracker.Clear();
        var contratos = await _context.Contratos.ToListAsync();
        contratos.Should().HaveCount(2);
    }

    [Fact]
    public async Task RollbackAsync_DeveExecutarSemErro()
    {
        var contrato = new ContratoBuilder().Build();
        await _context.Contratos.AddAsync(contrato);

        await _unitOfWork.RollbackAsync();

        var count = await _context.Contratos.CountAsync();
        count.Should().Be(0);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _unitOfWork.Dispose();
    }
}
