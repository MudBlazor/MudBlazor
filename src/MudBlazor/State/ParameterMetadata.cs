using System.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents information about a parameter.
/// </summary>
[DebuggerDisplay("ParameterName = {ParameterName}")]
internal class ParameterMetadata
{
    /// <summary>
    /// Gets the associated parameter name of the component's <see cref="ParameterAttribute"/>.
    /// </summary>
    public string ParameterName { get; }

    public string? ComparerParameterName { get; }

    /// <summary>
    /// Gets the unique name of the handler.
    /// </summary>
    /// <remarks>
    /// This is used to identify the handler uniquely.
    /// A <c>null</c> value indicates that the handler is always unique and typically means the usage of an anonymous function (lambda expression) as the handler instead of a method group.
    /// If two handlers have the same name, they are considered identical; otherwise, they are considered distinct.
    /// </remarks>
    public string? HandlerName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterMetadata"/> class with the specified handler name.
    /// </summary>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="handlerName">The handler's name.</param>
    public ParameterMetadata(string parameterName, string? handlerName)
    {
        ParameterName = parameterName;
        HandlerName = handlerName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterMetadata"/> class with the specified handler name.
    /// </summary>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="handlerName">The handler's name.</param>
    /// <param name="comparerParameterName"></param>
    public ParameterMetadata(string parameterName, string? handlerName, string? comparerParameterName)
        : this(parameterName, handlerName)
    {
        ComparerParameterName = comparerParameterName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterMetadata"/> class with the specified handler name.
    /// </summary>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    public ParameterMetadata(string parameterName)
        : this(parameterName, null)
    {
    }

    public override string ToString() => ParameterName;
}
