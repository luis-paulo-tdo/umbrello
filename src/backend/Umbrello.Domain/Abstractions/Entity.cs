namespace Umbrello.Domain.Abstractions;

public abstract class Entity<TId> : IEquatable<Entity<TId>> where TId : notnull
{
    public TId Id { get; protected init; }

    protected Entity(TId id) => Id = id;

    /// <summary>Construtor sem parâmetros exigido pelo EF Core para materialização.</summary>
    protected Entity() => Id = default!;

    public bool Equals(Entity<TId>? other) => other is not null && other.GetType() == GetType() && other.Id.Equals(Id);

    public override bool Equals(object? obj) => obj is Entity<TId> entity && Equals(entity);

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);
}
