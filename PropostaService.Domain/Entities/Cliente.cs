using PropostaService.Domain.ValueObjects;

namespace PropostaService.Domain.Entities;

public class Cliente
{
    public string Nome { get; private set; }
    public Cpf Cpf { get; private set; }

    protected Cliente()
    {
        Nome = string.Empty;
        Cpf = null!;
    }

    private Cliente(string nome, Cpf cpf)
    {
        Nome = nome;
        Cpf = cpf;
    }

    public static Cliente Criar(string nome, string cpf)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do cliente não pode ser vazio", nameof(nome));

        if (nome.Length < 3 || nome.Length > 200)
            throw new ArgumentException("Nome do cliente deve ter entre 3 e 200 caracteres", nameof(nome));

        var cpfObj = Cpf.Criar(cpf);
        return new Cliente(nome.Trim(), cpfObj);
    }

    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome do cliente não pode ser vazio", nameof(nome));

        if (nome.Length < 3 || nome.Length > 200)
            throw new ArgumentException("Nome do cliente deve ter entre 3 e 200 caracteres", nameof(nome));

        Nome = nome.Trim();
    }
}
