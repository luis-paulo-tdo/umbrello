using Umbrello.Domain.Abstractions;
using Umbrello.Domain.Enums;

namespace Umbrello.Domain.ValueObjetcs;

public sealed class Preferences : ValueObject
{
    public TimeZoneId TimeZone { get; }
    public NotificationTime NotificationTime { get; }
    public Units Units { get; }
    public bool NotificationsEnabled { get; }

    private Preferences(TimeZoneId timeZone, NotificationTime notificationTime, Units units, bool notificationsEnabled)
    {
        TimeZone = timeZone;
        NotificationTime = notificationTime;
        Units = units;
        NotificationsEnabled = notificationsEnabled;
    }

    public static Result<Preferences> Create(string timeZoneId, TimeOnly notificationTime, Units units, bool notificationsEnabled)
    {
        var timeZone = TimeZoneId.Create(timeZoneId);

        if (timeZone.IsFailure)
        {
            return Result.Failure<Preferences>(timeZone.Error);
        }

        var time = NotificationTime.Create(notificationTime);

        if (time.IsFailure)
        {
            return Result.Failure<Preferences>(time.Error);
        }

        return Result.Success(new Preferences(timeZone.Value, time.Value, units, notificationsEnabled));
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return TimeZone;
        yield return NotificationTime;
        yield return Units;
        yield return NotificationsEnabled;
    }
}
