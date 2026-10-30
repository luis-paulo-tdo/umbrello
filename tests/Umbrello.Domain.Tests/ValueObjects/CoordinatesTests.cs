using Shouldly;
using Umbrello.Domain.Errors;
using Umbrello.Domain.ValueObjetcs;

namespace Umbrello.Domain.Tests.ValueObjects;

public class CoordinatesTests
{
    [Theory]
    [InlineData(-23.550520, -46.633308)]
    [InlineData(90, 180)]
    [InlineData(-90, -180)]
    public void Create_ValidValues_Succeds(decimal latitude, decimal longitude)
    {
        Coordinates.Create(latitude, longitude).IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Create_LatitudeAboveRange_Fails()
    {
        var result = Coordinates.Create(90.1m, 0m);

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CoordinatesErrors.LatitudeOutOfRange);
    }

    [Fact]
    public void Create_RoundsToFourDecimalPlaces()
    {
        Coordinates.Create(-23.5505206m, -46.6333094m).Value.Latitude.ShouldBe(-23.5505m);
    }

    [Fact]
    public void Create_NearbyCoordinates_AreEqual()
    {
        var first = Coordinates.Create(-23.5505206m, -46.6333094m).Value;
        var second = Coordinates.Create(-23.55052m, -46.63331m).Value;

        first.ShouldBe(second);
    }
}
