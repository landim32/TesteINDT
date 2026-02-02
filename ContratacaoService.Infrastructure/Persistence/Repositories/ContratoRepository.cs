using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Interfaces.Repositories;
using ContratacaoService.Domain.ValueObjects;
using ContratacaoService.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Persistence.Repositories;

public class ContratoRepository : IContratoRepository
{
    private readonly ContratacaoDbContext _context;

    public ContratoRepository(ContratacaoDbContext context)
    {
        _context = context;
    }

    public async Task<Contrato?> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Contratos
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Contrato?> ObterPorPropostaIdAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        var contratos = await _context.Contratos
            .FromSqlRaw("SELECT * FROM \"Contratos\" WHERE \"PropostaId\" = {0}", propostaId)
            .ToListAsync(cancellationToken);
        
        return contratos.FirstOrDefault();
    }

    public async Task<IEnumerable<Contrato>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Contratos
            .OrderByDescending(c => c.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task AdicionarAsync(Contrato contrato, CancellationToken cancellationToken = default)
    {
        await _context.Contratos.AddAsync(contrato, cancellationToken);
    }

    public Task AtualizarAsync(Contrato contrato, CancellationToken cancellationToken = default)
    {
        _context.Contratos.Update(contrato);
        return Task.CompletedTask;
    }

    public async Task<bool> ExisteContratoParaPropostaAsync(Guid propostaId, CancellationToken cancellationToken = default)
    {
        var contratos = await _context.Contratos
            .FromSqlRaw("SELECT * FROM \"Contratos\" WHERE \"PropostaId\" = {0}", propostaId)
            .ToListAsync(cancellationToken);
        
        return contratos.Any();
    }
}
