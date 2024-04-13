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
    private readonly IParameterSetRegister _parameterSetRegister;
    private readonly List<ISmartParameterAttachable> _smartParameterAttachables;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRegistrationBuilderScope"/> class with the specified parameter set register.
    /// </summary>
    /// <param name="parameterSetRegister">The <see cref="IParameterSetRegister"/> used to register the parameter during the <see cref="Attach"/>.</param>
    public ParameterRegistrationBuilderScope(IParameterSetRegister parameterSetRegister)
    {
        _parameterSetRegister = parameterSetRegister;
        _smartParameterAttachables = new List<ISmartParameterAttachable>();
    }

    /// <summary>
    /// Creates a parameter builder for registering a parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
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
        _smartParameterAttachables.Add(builder);

        return builder;
    }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        foreach (var builder in _smartParameterAttachables)
        {
            builder.Attach();
        }
    }
}
