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
    private readonly IParameterStatesWriter _parameterStatesWriter;
    private readonly IParameterScopeContainer _parameterScopeContainer;

    /// <summary>
    /// Gets a value indicating whether the parameter registration builder scope is locked.
    /// </summary>
    /// <remarks>
    /// The scope becomes locked when it has ended (Disposed), indicating that no more parameter states will be registered.
    /// </remarks>
    public bool IsLocked => _parameterScopeContainer.IsLocked;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterRegistrationBuilderScope"/> class with the specified parameter set register.
    /// </summary>
    public ParameterRegistrationBuilderScope(IParameterScopeContainer scopeContainer, IParameterStatesWriter parameterStatesWriter)
    {
        _parameterScopeContainer = scopeContainer;
        _parameterStatesWriter = parameterStatesWriter;
    }

    /// <inheritdoc/>
    public RegisterParameterBuilder<T> RegisterParameter<T>(string parameterName) => RegisterParameter<T>().WithName(parameterName);

    /// <inheritdoc/>
    public RegisterParameterBuilder<T> RegisterParameter<T>()
    {
        var builder = new RegisterParameterBuilder<T>();
        _parameterStatesWriter.WriteParameter(builder);

        return builder;
    }

    /// <inheritdoc/>
    void IDisposable.Dispose() => _parameterScopeContainer.Dispose();

    /// <summary>
    /// Provides functionality to process parameter states, including reading and writing parameter builders.
    /// </summary>
    internal class ParameterStatesProcessor : IParameterStatesReader, IParameterStatesWriter
    {
        private readonly List<IParameterBuilderAttach> _builders = new();

        /// <inheritdoc />
        void IParameterStatesWriter.WriteParameter(IParameterBuilderAttach builder) => _builders.Add(builder);

        /// <inheritdoc />
        IEnumerable<IParameterComponentLifeCycle> IParameterStatesReader.ReadParameters() => _builders.Select(parameter => parameter.Attach());

        /// <inheritdoc />
        void IParameterStatesReader.Complete() => CleanUp();

        /// <summary>
        /// Clears the list of parameter builders.
        /// </summary>
        private void CleanUp()
        {
            _builders.Clear();
            _builders.TrimExcess();
        }
    }
}
