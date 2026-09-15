using System.Reflection;

namespace Umbrello.Architecture.Tests;

public class LayerDependencyTests
{
    private static readonly Assembly Domain = typeof(Umbrello.Domain.AssemblyReference).Assembly;
    private static readonly Assembly Contracts = typeof(Umbrello.Contracts.AssemblyReference).Assembly;
    private static readonly Assembly Application = typeof(Umbrello.Application.AssemblyReference).Assembly;
    private static readonly Assembly Infrastructure = typeof(Umbrello.Infrastructure.AssemblyReference).Assembly;
    private static readonly Assembly Messaging = typeof(Umbrello.Messaging.RabbitMq.AssemblyReference).Assembly;
}
