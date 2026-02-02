using ContratacaoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContratacaoService.Infrastructure.Persistence.Context;

public class ContratacaoDbContext : DbContext
{
    public ContratacaoDbContext(DbContextOptions<ContratacaoDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contrato> Contratos => Set<Contrato>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContratacaoDbContext).Assembly);
    }
}
