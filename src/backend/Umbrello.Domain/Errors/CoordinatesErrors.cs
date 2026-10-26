using Umbrello.Domain.Abstractions;

namespace Umbrello.Domain.Errors;

public static class CoordinatesErrors
{
    public static readonly Error LatitudeOutOfRange =
        new("Coordinates.LatitudeOutOfRange", "A latitude precisa estar entre -90 e 90.");

    public static readonly Error LongitudeOutOfRange =
        new("Coordinates.LongitudeOutOfRange", "A latitude precisa estar entre -180 e 180.");
}
