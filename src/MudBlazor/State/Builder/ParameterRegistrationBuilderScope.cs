// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Represents a scope for registering parameters.
/// </summary>
internal class ParameterRegistrationBuilderScope : IDisposable
{
    private readonly List<ISmartAttachable> _builders;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRegistrationBuilderScope"/> class with the specified parameter set register.
    /// </summary>
    public ParameterRegistrationBuilderScope()
    {
        _builders = new List<ISmartAttachable>();
    }

    /// <summary>
    /// Creates a parameter builder for registering a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <returns>A parameter builder for registering a parameter of the specified type.</returns>
    public RegisterParameterBuilder<T> CreateParameterBuilder<T>(string parameterName)
    {
        return CreateParameterBuilder<T>().WithName(parameterName);
    }

    /// <summary>
    /// Creates a parameter builder for registering a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <returns>A parameter builder for registering a parameter of the specified type.</returns>
    public RegisterParameterBuilder<T> CreateParameterBuilder<T>()
    {
        var builder = new RegisterParameterBuilder<T>();
        _builders.Add(builder);

        return builder;
    }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        foreach (var builder in _builders)
        {
            builder.Attach();
        }
    }
}
