using ContratacaoService.Domain.Enums;
using ContratacaoService.Domain.ValueObjects;

namespace ContratacaoService.Domain.Entities;

public class Contrato
{
    public Guid Id { get; private set; }
    public PropostaId PropostaId { get; private set; }
    public DateTime DataContratacao { get; private set; }
    public StatusContrato Status { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataAtualizacao { get; private set; }

    private Contrato() { }

    public Contrato(PropostaId propostaId)
    {
        Id = Guid.NewGuid();
        PropostaId = propostaId ?? throw new ArgumentNullException(nameof(propostaId));
        DataContratacao = DateTime.UtcNow;
        Status = StatusContrato.Ativo;
        DataCriacao = DateTime.UtcNow;
    }

    public void Cancelar()
    {
        if (Status == StatusContrato.Cancelado)
            throw new InvalidOperationException("Contrato já está cancelado");

        Status = StatusContrato.Cancelado;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Suspender()
    {
        if (Status != StatusContrato.Ativo)
            throw new InvalidOperationException("Apenas contratos ativos podem ser suspensos");

        Status = StatusContrato.Suspenso;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Reativar()
    {
        if (Status != StatusContrato.Suspenso)
            throw new InvalidOperationException("Apenas contratos suspensos podem ser reativados");

        Status = StatusContrato.Ativo;
        DataAtualizacao = DateTime.UtcNow;
    }

    public void Expirar()
    {
        if (Status == StatusContrato.Cancelado)
            throw new InvalidOperationException("Contrato cancelado não pode expirar");

        Status = StatusContrato.Expirado;
        DataAtualizacao = DateTime.UtcNow;
    }
}
