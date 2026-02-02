using PropostaService.Domain.Enums;
using PropostaService.Domain.Events;
using PropostaService.Domain.Exceptions;

namespace PropostaService.Domain.Entities;

public class Proposta
{
    private readonly List<object> _domainEvents = new();

    public Guid Id { get; private set; }
    public Cliente Cliente { get; private set; }
    public Seguro Seguro { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public StatusProposta Status { get; private set; }

    public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

    protected Proposta()
    {
        Id = Guid.Empty;
        Cliente = null!;
        Seguro = null!;
        DataCriacao = DateTime.UtcNow;
        Status = StatusProposta.EmAnalise;
    }

    private Proposta(Cliente cliente, Seguro seguro)
    {
        Id = Guid.NewGuid();
        Cliente = cliente;
        Seguro = seguro;
        DataCriacao = DateTime.UtcNow;
        Status = StatusProposta.EmAnalise;
    }

    public static Proposta Criar(string nomeCliente, string cpf, string tipoSeguro, 
        decimal valorCobertura, decimal valorPremio)
    {
        var cliente = Cliente.Criar(nomeCliente, cpf);
        var seguro = Seguro.Criar(tipoSeguro, valorCobertura, valorPremio);

        var proposta = new Proposta(cliente, seguro);
        proposta.AdicionarEvento(new PropostaCriadaEvent(proposta.Id, cliente.Nome, cliente.Cpf.Valor));

        return proposta;
    }

    public void Aprovar()
    {
        if (Status == StatusProposta.Aprovada)
            throw new PropostaInvalidaException("Proposta já foi aprovada");

        if (Status == StatusProposta.Rejeitada)
            throw new PropostaInvalidaException("Proposta rejeitada não pode ser aprovada");

        Status = StatusProposta.Aprovada;
        AdicionarEvento(new PropostaAprovadaEvent(Id, Cliente.Nome));
    }

    public void Rejeitar()
    {
        if (Status == StatusProposta.Rejeitada)
            throw new PropostaInvalidaException("Proposta já foi rejeitada");

        if (Status == StatusProposta.Aprovada)
            throw new PropostaInvalidaException("Proposta aprovada não pode ser rejeitada");

        Status = StatusProposta.Rejeitada;
        AdicionarEvento(new PropostaRejeitadaEvent(Id, Cliente.Nome));
    }

    public void AlterarStatus(StatusProposta novoStatus)
    {
        if (novoStatus == Status)
            return;

        switch (novoStatus)
        {
            case StatusProposta.Aprovada:
                Aprovar();
                break;
            case StatusProposta.Rejeitada:
                Rejeitar();
                break;
            case StatusProposta.EmAnalise:
                if (Status != StatusProposta.EmAnalise)
                    throw new PropostaInvalidaException("Não é possível retornar proposta para análise");
                break;
        }
    }

    public void AtualizarCliente(string nomeCliente)
    {
        if (Status == StatusProposta.Aprovada)
            throw new PropostaInvalidaException("Não é possível atualizar proposta aprovada");

        if (Status == StatusProposta.Rejeitada)
            throw new PropostaInvalidaException("Não é possível atualizar proposta rejeitada");

        Cliente.AtualizarNome(nomeCliente);
    }

    public void AtualizarSeguro(decimal valorCobertura, decimal valorPremio)
    {
        if (Status == StatusProposta.Aprovada)
            throw new PropostaInvalidaException("Não é possível atualizar proposta aprovada");

        if (Status == StatusProposta.Rejeitada)
            throw new PropostaInvalidaException("Não é possível atualizar proposta rejeitada");

        Seguro.AtualizarValores(valorCobertura, valorPremio);
    }

    private void AdicionarEvento(object evento)
    {
        _domainEvents.Add(evento);
    }

    public void LimparEventos()
    {
        _domainEvents.Clear();
    }
}
