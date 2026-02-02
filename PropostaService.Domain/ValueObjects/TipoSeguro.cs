using PropostaService.Domain.Exceptions;

namespace PropostaService.Domain.ValueObjects;

public sealed class TipoSeguro : IEquatable<TipoSeguro>
{
    public string Valor { get; private set; }

    private TipoSeguro()
    {
        Valor = string.Empty;
    }

    private TipoSeguro(string valor)
    {
        Valor = valor;
    }

    public static TipoSeguro Criar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Tipo de seguro não pode ser vazio");

        if (valor.Length < 3 || valor.Length > 50)
            throw new DomainException("Tipo de seguro deve ter entre 3 e 50 caracteres");

        return new TipoSeguro(valor.Trim());
    }

    public static TipoSeguro Vida => new("Vida");
    public static TipoSeguro Auto => new("Auto");
    public static TipoSeguro Residencial => new("Residencial");
    public static TipoSeguro Saude => new("Saúde");

    public bool Equals(TipoSeguro? other)
    {
        if (other is null) return false;
        return Valor.Equals(other.Valor, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TipoSeguro);
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return Valor;
    }

    public static bool operator ==(TipoSeguro? left, TipoSeguro? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(TipoSeguro? left, TipoSeguro? right)
    {
        return !(left == right);
    }
}
