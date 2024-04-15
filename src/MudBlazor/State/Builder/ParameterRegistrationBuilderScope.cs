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
internal class ParameterRegistrationBuilderScope : IParameterRegistrationBuilderScope, IParameterStatesReader
{
    private readonly Action? _onScopeEndedAction;
    private readonly List<IParameterBuilderAttach> _builders;

    /// <summary>
    /// Gets a value indicating whether the parameter registration builder scope is locked.
    /// </summary>
    /// <remarks>
    /// The scope becomes locked when it has ended (Disposed), indicating that no more parameter states will be registered.
    /// </remarks>
    public bool IsLocked { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRegistrationBuilderScope"/> class with the specified parameter set register.
    /// </summary>
    /// <param name="onScopeEndedAction">The action to be executed when the scope ends.</param>
    public ParameterRegistrationBuilderScope(Action? onScopeEndedAction = null)
    {
        _onScopeEndedAction = onScopeEndedAction;
        _builders = new List<IParameterBuilderAttach>();
    }

    /// <inheritdoc/>
    public RegisterParameterBuilder<T> RegisterParameter<T>(string parameterName) => RegisterParameter<T>().WithName(parameterName);

    /// <inheritdoc/>
    public RegisterParameterBuilder<T> RegisterParameter<T>()
    {
        var builder = new RegisterParameterBuilder<T>();
        _builders.Add(builder);

        return builder;
    }

    /// <summary>
    /// Clears the list of parameter builders.
    /// </summary>
    private void CleanUp()
    {
        _builders.Clear();
        _builders.TrimExcess();
    }

    /// <inheritdoc/>
    void IDisposable.Dispose()
    {
        if (!IsLocked)
        {
            IsLocked = true;
            _onScopeEndedAction?.Invoke();
        }
    }

    /// <inheritdoc />
    IEnumerable<IParameterComponentLifeCycle> IParameterStatesReader.ReadParameters() => _builders.Select(parameter => parameter.Attach());

    /// <inheritdoc />
    void IParameterStatesReader.Complete() => CleanUp();
}
