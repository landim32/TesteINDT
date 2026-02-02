using PropostaService.Domain.Interfaces.Repositories;
using PropostaService.Infrastructure.Persistence.Context;

namespace PropostaService.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly PropostaDbContext _context;

    public UnitOfWork(PropostaDbContext context)
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
}
