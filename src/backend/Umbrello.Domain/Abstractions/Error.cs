namespace Umbrello.Domain.Abstractions;

/// <summary>
/// Erro de negócio identificável por código, para mapear em ProblemDetails.
/// </summary>
public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("General.Null", "O valor informado é nulo.");
}
