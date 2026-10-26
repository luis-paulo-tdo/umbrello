using Umbrello.Domain.Abstractions;

namespace Umbrello.Domain.Errors;

public static class TimedZoneIdErrors
{
    public static readonly Error Empty = new("TimedZoneId.Empty", "O fuso horário é obrigatório.");
    public static readonly Error NotFound = new("TimedZoneId.NotFound", "Fuso horário desconhecido.");
}
