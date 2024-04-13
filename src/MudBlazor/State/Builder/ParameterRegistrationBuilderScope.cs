// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Represents a scope for registering parameters.
/// </summary>
internal class ParameterRegistrationBuilderScope : IParameterRegistrationBuilderScope
{
    private bool _isLocked;
    private readonly IParameterStatesFactoryWriter _parameterStatesFactoryWriter;
    private readonly List<IParameterBuilderAttach> _builders;

    /// <summary>
    /// Gets a value indicating whether the parameter registration builder scope is locked.
    /// </summary>
    /// <remarks>
    /// The scope becomes locked when it has ended (Disposed), indicating that no more parameter states will be registered.
    /// </remarks>
    public bool IsLocked => _isLocked;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRegistrationBuilderScope"/> class with the specified parameter set register.
    /// </summary>
    /// <param name="parameterStatesFactoryWriter">The <see cref="IParameterStatesFactoryWriter"/> used to register the parameters during the end of the scope.</param>
    public ParameterRegistrationBuilderScope(IParameterStatesFactoryWriter parameterStatesFactoryWriter)
    {
        _builders = new List<IParameterBuilderAttach>();
        _parameterStatesFactoryWriter = parameterStatesFactoryWriter;
    }

    /// <inheritdoc/>
    public RegisterParameterBuilder<T> CreateParameterBuilder<T>(string parameterName)
    {
        return CreateParameterBuilder<T>().WithName(parameterName);
    }

    /// <inheritdoc/>
    public RegisterParameterBuilder<T> CreateParameterBuilder<T>()
    {
        var builder = new RegisterParameterBuilder<T>();
        _builders.Add(builder);

        return builder;
    }

    /// <summary>
    /// Clears the list of parameter builders.
    /// </summary>
    public void CleanUp() => _builders.Clear();

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        if (!_isLocked)
        {
            _isLocked = true;
            try
            {
                _parameterStatesFactoryWriter.WriteParameters(_builders.Select(parameter => parameter.Attach()));
            }
            finally
            {
                _parameterStatesFactoryWriter.Close();
            }
        }
    }
}
