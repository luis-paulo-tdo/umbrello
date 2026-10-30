using Umbrello.Domain.Abstractions;
using Umbrello.Domain.Errors;

namespace Umbrello.Domain.ValueObjetcs;

/// <summary>
/// Horário local escolhido para o envio diário.
/// Restrito a múltiplos de 5 minutos, porque o job de disparo roda a cada 5 minutos (RN04).
/// </summary>
public sealed class NotificationTime : ValueObject
{
    public const int GranularityInMinutes = 5;
    public TimeOnly Value { get; }

    private NotificationTime(TimeOnly value) => Value = value;

    public static Result<NotificationTime> Create(TimeOnly value)
    {
        if (value.Minute % GranularityInMinutes != 0 || value.Second != 0 || value.Millisecond != 0)
        {
            return Result.Failure<NotificationTime>(NotificationTimeErrors.NotOnFiveMinuteBoundary);
        }

        return Result.Success(new NotificationTime(value));
    }

    public override string ToString() => Value.ToString("HH:mm", null);

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}
