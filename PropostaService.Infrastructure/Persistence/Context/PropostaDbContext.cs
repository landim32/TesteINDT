using Microsoft.EntityFrameworkCore;
using PropostaService.Domain.Entities;
using PropostaService.Infrastructure.Persistence.Configurations;

namespace PropostaService.Infrastructure.Persistence.Context;

public class PropostaDbContext : DbContext
{
    public PropostaDbContext(DbContextOptions<PropostaDbContext> options) : base(options)
    {
    }

    public DbSet<Proposta> Propostas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new PropostaConfiguration());

        modelBuilder.HasDefaultSchema("propostas");
    }
}
