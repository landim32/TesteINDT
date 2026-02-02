using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.Interfaces.Repositories;
using PropostaService.Infrastructure.Persistence.Context;

namespace PropostaService.Infrastructure.Persistence.Repositories;

public class PropostaRepository : IPropostaRepository
{
    private readonly PropostaDbContext _context;

    public PropostaRepository(PropostaDbContext context)
    {
        _context = context;
    }

    public async Task<Proposta?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Propostas
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Proposta>> ObterTodasAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Propostas
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Proposta>> ObterPorStatusAsync(StatusProposta status, CancellationToken cancellationToken = default)
    {
        return await _context.Propostas
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Proposta proposta, CancellationToken cancellationToken = default)
    {
        await _context.Propostas.AddAsync(proposta, cancellationToken);
    }

    public Task AtualizarAsync(Proposta proposta, CancellationToken cancellationToken = default)
    {
        _context.Propostas.Update(proposta);
        return Task.CompletedTask;
    }
}
