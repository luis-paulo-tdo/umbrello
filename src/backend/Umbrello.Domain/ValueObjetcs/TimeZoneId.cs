using Umbrello.Domain.Abstractions;
using Umbrello.Domain.Errors;

namespace Umbrello.Domain.ValueObjetcs;

/// <summary>
/// Identificador de fuso horário contra a base de fusos do sistema.
/// </summary>
public sealed class TimeZoneId : ValueObject
{
    public string Value { get; }

    private TimeZoneId(string value) => Value = value;

    public static Result<TimeZoneId> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<TimeZoneId>(TimedZoneIdErrors.Empty);
        }

        var trimmed = value.Trim();

        return TimeZoneInfo.TryFindSystemTimeZoneById(trimmed, out _)
            ? Result.Success(new TimeZoneId(trimmed)) : Result.Failure<TimeZoneId>(TimedZoneIdErrors.NotFound);
    }

    public TimeZoneInfo ToTimeZoneInfo() => TimeZoneInfo.FindSystemTimeZoneById(Value);

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}
