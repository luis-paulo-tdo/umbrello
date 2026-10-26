using Umbrello.Domain.Abstractions;

namespace Umbrello.Domain.Errors;

public static class NotificationTimeErrors
{
    public static readonly Error NotOnFiveMinuteBoundary =
        new("NotificationTime.NotOnFiveMinuteBoundary", "O horário de notificação precisa estar em múltiplos de 5 minutos.");
}
