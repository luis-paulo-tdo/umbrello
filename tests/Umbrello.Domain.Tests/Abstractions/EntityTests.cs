using Shouldly;

namespace Umbrello.Domain.Abstractions;

public class EntityTests
{
    private sealed class Customer(Guid id) : Entity<Guid>(id);

    private sealed class Product(Guid id) : Entity<Guid>(id);

    [Fact]
    public void Equals_SameIdAndType_ReturnsTrue()
    {
        var id = Guid.NewGuid();

        new Customer(id).ShouldBe(new Customer(id));
    }

    [Fact]
    public void Equals_SameIdDifferentType_ReturnsFalse()
    {
        var id = Guid.NewGuid();

        new Customer(id).Equals(new Product(id)).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentId_ReturnsFalse()
    {
        new Customer(Guid.NewGuid()).ShouldNotBe(new Customer(Guid.NewGuid()));
    }
}
