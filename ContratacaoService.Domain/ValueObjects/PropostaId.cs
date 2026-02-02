namespace ContratacaoService.Domain.ValueObjects;

public class PropostaId : IEquatable<PropostaId>
{
    public Guid Value { get; }

    public PropostaId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("PropostaId não pode ser vazio", nameof(value));

        Value = value;
    }

    public static implicit operator Guid(PropostaId propostaId) => propostaId.Value;
    public static implicit operator PropostaId(Guid value) => new(value);

    public bool Equals(PropostaId? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value);
    }

    public override bool Equals(object? obj)
    {
        return obj is PropostaId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static bool operator ==(PropostaId? left, PropostaId? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Equals(right);
    }

    public static bool operator !=(PropostaId? left, PropostaId? right)
    {
        return !(left == right);
    }
}
