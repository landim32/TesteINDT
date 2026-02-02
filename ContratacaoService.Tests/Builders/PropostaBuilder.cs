using ContratacaoService.Domain.Entities;
using ContratacaoService.Domain.Enums;

namespace ContratacaoService.Tests.Builders;

public class PropostaBuilder
{
    private Guid _id = Guid.NewGuid();
    private string _nomeCliente = "João da Silva";
    private string _cpf = "12345678909";
    private string _tipoSeguro = "Auto";
    private decimal _valorCobertura = 50000;
    private decimal _valorPremio = 1000;
    private StatusProposta _status = StatusProposta.Aprovada;

    public PropostaBuilder ComId(Guid id)
    {
        _id = id;
        return this;
    }

    public PropostaBuilder ComNomeCliente(string nomeCliente)
    {
        _nomeCliente = nomeCliente;
        return this;
    }

    public PropostaBuilder ComCpf(string cpf)
    {
        _cpf = cpf;
        return this;
    }

    public PropostaBuilder ComTipoSeguro(string tipoSeguro)
    {
        _tipoSeguro = tipoSeguro;
        return this;
    }

    public PropostaBuilder ComValorCobertura(decimal valorCobertura)
    {
        _valorCobertura = valorCobertura;
        return this;
    }

    public PropostaBuilder ComValorPremio(decimal valorPremio)
    {
        _valorPremio = valorPremio;
        return this;
    }

    public PropostaBuilder ComStatus(StatusProposta status)
    {
        _status = status;
        return this;
    }

    public Proposta Build()
    {
        return new Proposta
        {
            Id = _id,
            NomeCliente = _nomeCliente,
            Cpf = _cpf,
            TipoSeguro = _tipoSeguro,
            ValorCobertura = _valorCobertura,
            ValorPremio = _valorPremio,
            DataCriacao = DateTime.UtcNow,
            Status = _status
        };
    }

    public Proposta BuildEmAnalise()
    {
        _status = StatusProposta.EmAnalise;
        return Build();
    }

    public Proposta BuildAprovada()
    {
        _status = StatusProposta.Aprovada;
        return Build();
    }

    public Proposta BuildRejeitada()
    {
        _status = StatusProposta.Rejeitada;
        return Build();
    }
}
