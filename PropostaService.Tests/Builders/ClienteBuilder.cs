using PropostaService.Domain.Entities;

namespace PropostaService.Tests.Builders;

public class ClienteBuilder
{
    private string _nome = "João da Silva";
    private string _cpf = "12345678909";

    public ClienteBuilder ComNome(string nome)
    {
        _nome = nome;
        return this;
    }

    public ClienteBuilder ComCpf(string cpf)
    {
        _cpf = cpf;
        return this;
    }

    public Cliente Build()
    {
        return Cliente.Criar(_nome, _cpf);
    }
}
