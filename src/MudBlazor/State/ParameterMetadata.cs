﻿using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents information about a parameter.
/// </summary>
internal class ParameterMetadata
{
    /// <summary>
    /// Gets the associated parameter name of the component's <see cref="ParameterAttribute"/>.
    /// </summary>
    public string ParameterName { get; }

    /// <summary>
    /// Gets the unique name of the handler.
    /// </summary>
    /// <remarks>
    /// This is used to identify the handler uniquely.
    /// A <c>null</c> value indicates that the handler is always unique.
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
}
