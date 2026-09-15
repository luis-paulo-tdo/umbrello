using System.Reflection;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace Umbrello.Architecture.Tests;

public class LayerDependencyTests
{
    private static readonly Assembly Domain = typeof(Umbrello.Domain.AssemblyReference).Assembly;
    private static readonly Assembly Contracts = typeof(Umbrello.Contracts.AssemblyReference).Assembly;
    private static readonly Assembly Application = typeof(Umbrello.Application.AssemblyReference).Assembly;
    private static readonly Assembly Infrastructure = typeof(Umbrello.Infrastructure.AssemblyReference).Assembly;
    private static readonly Assembly Messaging = typeof(Umbrello.Messaging.RabbitMq.AssemblyReference).Assembly;

    [Fact]
    public void Domain_ShouldNotDependOnAnyOtherLayer()
    {
        var result = Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Umbrello.Contracts",
                "Umbrello.Application",
                "Umbrello.Infrastructure",
                "Umbrello.Messaging.RabbitMq",
                "Umbrello.Api"
            ).GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Domain_ShouldNotDependOnInfrastructureLibraries()
    {
        var result = Types.InAssembly(Domain)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "RabbitMQ.Client",
                "System.Data",
                "System.Net.Http"
            ).GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Contracts_ShouldNotDependOnAnyOtherLayer()
    {
        var result = Types.InAssembly(Contracts)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Umbrello.Domain",
                "Umbrello.Application",
                "Umbrello.Infrastructure",
                "Umbrello.Messaging.RabbitMq",
                "Umbrello.Api"
            ).GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureOrHosts()
    {
        var result = Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Umbrello.Infrastructure",
                "Umbrello.Messaging.RabbitMq",
                "Umbrello.Api"
            ).GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Application_ShouldNotDependOnInfrastructureLibraries()
    {
        var result = Types.InAssembly(Application)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore",
                "RabbitMQ.Client",
                "MailKit",
                "Quartz"
            ).GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnHostsOrTransport()
    {
        var result = Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Umbrello.Messaging.RabbitMq",
                "Umbrello.Api"
            ).GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    [Fact]
    public void Messaging_ShouldNotDependOnInfrastructureOrHosts()
    {
        var result = Types.InAssembly(Infrastructure)
            .ShouldNot()
            .HaveDependencyOnAny(
                "Umbrello.Infrastructure",
                "Umbrello.Api"
            ).GetResult();

        Assert.True(result.IsSuccessful, Describe(result));
    }

    private static string Describe(TestResult result) =>
        result.IsSuccessful ? string.Empty : "Tipos que violam a regra: " + string.Join(", ", result.FailingTypeNames ?? []);
}
