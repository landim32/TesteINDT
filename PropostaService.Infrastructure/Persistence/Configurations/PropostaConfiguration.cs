using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;
using PropostaService.Domain.ValueObjects;

namespace PropostaService.Infrastructure.Persistence.Configurations;

public class PropostaConfiguration : IEntityTypeConfiguration<Proposta>
{
    public void Configure(EntityTypeBuilder<Proposta> builder)
    {
        builder.ToTable("Propostas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedNever();

        builder.Property(p => p.DataCriacao)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion(
                v => v.ToString(),
                v => (StatusProposta)Enum.Parse(typeof(StatusProposta), v));

        builder.OwnsOne(p => p.Cliente, cliente =>
        {
            cliente.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("ClienteNome");

            cliente.OwnsOne(c => c.Cpf, cpf =>
            {
                cpf.Property(c => c.Valor)
                    .IsRequired()
                    .HasMaxLength(11)
                    .HasColumnName("ClienteCpf");
            });
        });

        builder.OwnsOne(p => p.Seguro, seguro =>
        {
            seguro.Property(s => s.Id)
                .HasColumnName("SeguroId");

            seguro.OwnsOne(s => s.Tipo, tipo =>
            {
                tipo.Property(t => t.Valor)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("TipoSeguro");
            });

            seguro.OwnsOne(s => s.ValorCobertura, valor =>
            {
                valor.Property(v => v.Valor)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("ValorCobertura");
            });

            seguro.OwnsOne(s => s.ValorPremio, valor =>
            {
                valor.Property(v => v.Valor)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)")
                    .HasColumnName("ValorPremio");
            });
        });

        builder.Ignore(p => p.DomainEvents);
    }
}
