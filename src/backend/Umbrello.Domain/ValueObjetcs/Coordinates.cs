using Umbrello.Domain.Abstractions;
using Umbrello.Domain.Errors;

namespace Umbrello.Domain.ValueObjetcs;

/// <summary>
/// Coordenada geográfica arredondada em 4 casas decimais (~11m).
/// Desta forma, a mesma cidade favoritada por usuários diferentes é resolvida no mesmo registro.
/// </summary>
public sealed class Coordinates : ValueObject
{
    public const int Precision = 4;
    public decimal Latitude { get; }
    public decimal Longitude { get; }

    private Coordinates(decimal latitude, decimal longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static Result<Coordinates> Create(decimal latitude, decimal longitude)
    {
        if (latitude is < -90m or > 90m)
        {
            return Result.Failure<Coordinates>(CoordinatesErrors.LatitudeOutOfRange);
        }

        if (longitude is < -180m or > 180m)
        {
            return Result.Failure<Coordinates>(CoordinatesErrors.LongitudeOutOfRange);
        }

        return Result.Success(new Coordinates(Round(latitude), Round(longitude)));
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Latitude;
        yield return Longitude;
    }

    public static decimal Round(decimal value) => Math.Round(value, Precision, MidpointRounding.AwayFromZero);
}
