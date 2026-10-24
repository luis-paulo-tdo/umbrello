using Shouldly;
using Umbrello.Domain.Abstractions;

namespace Umbrello.Domain.Tests.Abstractions;

public class ResultTests
{
    [Fact]
    public void Success_HasNoError()
    {
        var result = Result.Success();

        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
    }

    [Fact]
    public void Failure_WithoutError_Throws()
    {
        Should.Throw<ArgumentException>(() => Result.Failure(Error.None));
    }

    [Fact]
    public void Value_OnFailure_Throws()
    {
        var result = Result.Failure<string>(new Error("Test.Error", "Falhou."));

        Should.Throw<InvalidOperationException>(() => result.Value);
    }
}
