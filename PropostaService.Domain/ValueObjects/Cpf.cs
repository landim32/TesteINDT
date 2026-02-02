using PropostaService.Domain.Exceptions;

namespace PropostaService.Domain.ValueObjects;

public sealed class Cpf : IEquatable<Cpf>
{
    public string Valor { get; private set; }

    private Cpf()
    {
        Valor = string.Empty;
    }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static Cpf Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("CPF não pode ser vazio");

        var cpfLimpo = LimparCpf(valor);

        if (!ValidarCpf(cpfLimpo))
            throw new DomainException("CPF inválido");

        return new Cpf(cpfLimpo);
    }

    private static string LimparCpf(string cpf)
    {
        return cpf.Replace(".", "").Replace("-", "").Trim();
    }

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Length != 11)
            return false;

        if (!cpf.All(char.IsDigit))
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        var multiplicador1 = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        var multiplicador2 = new[] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf.Substring(0, 9);
        var soma = 0;

        for (var i = 0; i < 9; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

        var resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        var digito = resto.ToString();
        tempCpf += digito;
        soma = 0;

        for (var i = 0; i < 10; i++)
            soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

        resto = soma % 11;
        resto = resto < 2 ? 0 : 11 - resto;

        digito += resto.ToString();

        return cpf.EndsWith(digito);
    }

    public string FormatarCpf()
    {
        return $"{Valor.Substring(0, 3)}.{Valor.Substring(3, 3)}.{Valor.Substring(6, 3)}-{Valor.Substring(9, 2)}";
    }

    public bool Equals(Cpf? other)
    {
        if (other is null) return false;
        return Valor == other.Valor;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Cpf);
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }

    public override string ToString()
    {
        return Valor;
    }

    public static bool operator ==(Cpf? left, Cpf? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Cpf? left, Cpf? right)
    {
        return !(left == right);
    }
}
