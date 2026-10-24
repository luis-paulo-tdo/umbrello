using Shouldly;
using Umbrello.Domain.Abstractions;

namespace Umbrello.Domain.Tests.Abstractions;

public class ValueObjectTests
{
    private sealed class Money(decimal amount, string currency) : ValueObject
    {
        public decimal Amount { get; } = amount;
        public string Currency { get; } = currency;

        protected override IEnumerable<object?> GetAtomicValues()
        {
            yield return Amount;
            yield return Currency;
        }
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        new Money(10m, "BRL").ShouldBe(new Money(10, "BRL"));
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        new Money(10m, "BRL").ShouldNotBe(new Money(10m, "USD"));
    }

    [Fact]
    public void GetHashCode_SameValues_AreEqual()
    {
        new Money(10m, "BRL").GetHashCode().ShouldBe(new Money(10m, "BRL").GetHashCode());
    }
}
