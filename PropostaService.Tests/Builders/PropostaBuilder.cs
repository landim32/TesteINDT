using PropostaService.Domain.Entities;
using PropostaService.Domain.Enums;

namespace PropostaService.Tests.Builders;

public class PropostaBuilder
{
    private string _nomeCliente = "João da Silva";
    private string _cpf = "12345678909";
    private string _tipoSeguro = "Auto";
    private decimal _valorCobertura = 50000;
    private decimal _valorPremio = 1000;

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

    public Proposta Build()
    {
        return Proposta.Criar(
            _nomeCliente,
            _cpf,
            _tipoSeguro,
            _valorCobertura,
            _valorPremio
        );
    }

    public Proposta BuildAprovada()
    {
        var proposta = Build();
        proposta.Aprovar();
        return proposta;
    }

    public Proposta BuildRejeitada()
    {
        var proposta = Build();
        proposta.Rejeitar();
        return proposta;
    }
}
