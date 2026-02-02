using PropostaService.Domain.Exceptions;

namespace PropostaService.Domain.ValueObjects;

public sealed class Dinheiro : IEquatable<Dinheiro>
{
    public decimal Valor { get; private set; }

    private Dinheiro()
    {
        Valor = 0;
    }

    private Dinheiro(decimal valor)
    {
        Valor = valor;
    }

    public static Dinheiro Criar(decimal valor)
    {
        if (valor < 0)
            throw new DomainException("Valor monetário não pode ser negativo");

        return new Dinheiro(valor);
    }

    public static Dinheiro Zero => new(0);

    public Dinheiro Somar(Dinheiro outro)
    {
        return new Dinheiro(Valor + outro.Valor);
    }

    public Dinheiro Subtrair(Dinheiro outro)
    {
        var resultado = Valor - outro.Valor;
        if (resultado < 0)
            throw new DomainException("Resultado da subtração não pode ser negativo");

        return new Dinheiro(resultado);
    }

    public bool Equals(Dinheiro? other)
    {
        if (other is null) return false;
        return Valor == other.Valor;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Dinheiro);
    }

    public override int GetHashCode()
    {
        return Valor.GetHashCode();
    }

    public override string ToString()
    {
        return Valor.ToString("C2");
    }

    public static bool operator ==(Dinheiro? left, Dinheiro? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Dinheiro? left, Dinheiro? right)
    {
        return !(left == right);
    }

    public static bool operator >(Dinheiro left, Dinheiro right)
    {
        return left.Valor > right.Valor;
    }

    public static bool operator <(Dinheiro left, Dinheiro right)
    {
        return left.Valor < right.Valor;
    }

    public static bool operator >=(Dinheiro left, Dinheiro right)
    {
        return left.Valor >= right.Valor;
    }

    public static bool operator <=(Dinheiro left, Dinheiro right)
    {
        return left.Valor <= right.Valor;
    }
}
