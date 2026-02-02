using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContratacaoService.Infrastructure.Persistence.Configurations;

public class ContratoConfiguration : IEntityTypeConfiguration<Contrato>
{
    public void Configure(EntityTypeBuilder<Contrato> builder)
    {
        builder.ToTable("Contratos");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(c => c.PropostaId)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => new PropostaId(v));

        builder.Property(c => c.DataContratacao)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.DataCriacao)
            .IsRequired()
            .HasColumnType("timestamp without time zone")
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        builder.Property(c => c.DataAtualizacao)
            .HasColumnType("timestamp without time zone")
            .HasConversion(
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified) : (DateTime?)null,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null);

        builder.HasIndex(c => c.PropostaId)
            .IsUnique();
    }
}
