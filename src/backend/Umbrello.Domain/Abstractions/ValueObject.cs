namespace Umbrello.Domain.Abstractions;

public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>Componentes que definem a identidade estrutural deste objeto.</summary>
    protected abstract IEnumerable<object?> GetAtomicValues();

    public bool Equals(ValueObject? other) =>
        other is not null && other.GetType() == GetType() && GetAtomicValues().SequenceEqual(other.GetAtomicValues());

    public override bool Equals(object? obj) => obj is ValueObject other && Equals(other);

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var value in GetAtomicValues())
        {
            hash.Add(value);
        }

        return hash.ToHashCode();
    }

    public static bool operator ==(ValueObject? left, ValueObject? right) => Equals(left, right);

    public static bool operator !=(ValueObject? left, ValueObject? right) => !Equals(left, right);
}
