using ContratacaoService.Domain.Interfaces.Repositories;
using ContratacaoService.Infrastructure.Persistence.Context;

namespace ContratacaoService.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ContratacaoDbContext _context;

    public UnitOfWork(ContratacaoDbContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
