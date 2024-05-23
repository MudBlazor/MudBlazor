// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a collection of registered parameters.
/// This class is part of MudBlazor's ParameterState framework.
/// </summary>
/// <remarks>
/// For details and usage please read CONTRIBUTING.md
/// </remarks>
internal class ParameterSet : IEnumerable<IParameterComponentLifeCycle>, IParameterStatesReaderOwner
{
    private readonly IParameterStatesReader _parameterStatesReader;

#if NET8_0_OR_GREATER
    private readonly Lazy<FrozenDictionary<string, IParameterComponentLifeCycle>> _parameters;
#else
    private readonly Lazy<Dictionary<string, IParameterComponentLifeCycle>> _parameters;
#endif

    /// <summary>
    /// Gets a value indicating whether the parameter set has been initialized.
    /// </summary>
    /// <remarks>
    /// The parameter set is considered initialized once the inner dictionary of parameters has been created.
    /// </remarks>
    public bool IsInitialized => _parameters.IsValueCreated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterSet"/> class with the specified parameters.
    /// </summary>
    /// <param name="parameters">An optional array of parameters to initialize the set.</param>
    public ParameterSet(params IParameterComponentLifeCycle[] parameters)
        : this(new ParameterSetReadonlyEnumerable(parameters))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterSet"/> class with the specified parameters.
    /// </summary>
    /// <param name="parameters">An enumerable collection of parameters to initialize the set.</param>
    public ParameterSet(IEnumerable<IParameterComponentLifeCycle> parameters)
        : this(new ParameterSetReadonlyEnumerable(parameters))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterSet"/> class with the specified parameter states factory.
    /// </summary>
    /// <param name="parameterStatesReader">The factory used to read an enumerable collection of parameters to initialize the set.</param>
    public ParameterSet(IParameterStatesReader parameterStatesReader)
    {
        _parameterStatesReader = parameterStatesReader;
        _parameterStatesReader.SetOwner(this);
#if NET8_0_OR_GREATER
        _parameters = new Lazy<FrozenDictionary<string, IParameterComponentLifeCycle>>(ParametersFactory);
#else
        _parameters = new Lazy<Dictionary<string, IParameterComponentLifeCycle>>(ParametersFactory);
#endif
    }


#if NET8_0_OR_GREATER
    private FrozenDictionary<string, IParameterComponentLifeCycle> ParametersFactory()
    {
        var parameters = _parameterStatesReader.ReadParameters();
        var dictionary = parameters.ToFrozenDictionary(parameter => parameter.Metadata.ParameterName, parameter => parameter);
        _parameterStatesReader.Complete();

        return dictionary;
    }
#else
    private Dictionary<string, IParameterComponentLifeCycle> ParametersFactory()
    {
        var parameters = _parameterStatesReader.ReadParameters();
        var dictionary = parameters.ToDictionary(parameter => parameter.Metadata.ParameterName, parameter => parameter);
        _parameterStatesReader.Complete();

        return dictionary;
    }
#endif

    /// <inheritdoc/>
    public void ForceParametersAttachment() => _ = _parameters.Value;

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnInitialized"/> for all registered parameters.
    /// </summary>
    public void OnInitialized()
    {
        foreach (var parameter in _parameters.Value.Values)
        {
            parameter.OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="IParameterComponentLifeCycle.OnParametersSet"/> for all registered parameters.
    /// </summary>
    public void OnParametersSet()
    {
        foreach (var parameter in _parameters.Value.Values)
        {
            parameter.OnParametersSet();
        }
    }

    /// <summary>
    /// Determines which <see cref="ParameterState{T}"/> have been changed and calls their respective change handler.
    /// </summary>
    /// <param name="baseSetParametersAsync">A func to call the base class' <see cref="ComponentBase.SetParametersAsync"/>.</param>
    /// <param name="parameters">The ParameterView coming from Blazor's  <see cref="ComponentBase.SetParametersAsync"/>.</param>
    public async Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
#if NET8_0_OR_GREATER
        var parametersHandlerShouldFire = _parameters.Value.Values
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToFrozenSet(ParameterHandlerUniquenessComparer.Default);
#else
        var parametersHandlerShouldFire = _parameters.Value.Values
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToHashSet(ParameterHandlerUniquenessComparer.Default);
#endif

        await baseSetParametersAsync(parameters);

        foreach (var parameterHandlerShouldFire in parametersHandlerShouldFire)
        {
            await parameterHandlerShouldFire.ParameterChangeHandleAsync();
        }
    }

    /// <summary>Searches the set for a given value and returns the equal value it finds, if any.</summary>
    /// <param name="parameterName">The value to search for.</param>
    /// <param name="parameterComponentLifeCycle">The value from the set that the search found, or the default value when the search yielded no match.</param>
    /// <returns>A value indicating whether the search was successful.</returns>
    public bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle)
    {
        return _parameters.Value.TryGetValue(parameterName, out parameterComponentLifeCycle);
    }

    /// <inheritdoc/>
    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => ((IReadOnlyDictionary<string, IParameterComponentLifeCycle>)_parameters.Value).Values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Represents an enumerable reader for parameter states.
    /// </summary>
    private class ParameterSetReadonlyEnumerable : IParameterStatesReader
    {
        private readonly IEnumerable<IParameterComponentLifeCycle> _parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterSetReadonlyEnumerable"/> class with the specified parameters.
        /// </summary>
        /// <param name="parameters">The parameters to be read.</param>
        public ParameterSetReadonlyEnumerable(IEnumerable<IParameterComponentLifeCycle> parameters) => _parameters = parameters;

        /// <inheritdoc />
        public void SetOwner(IParameterStatesReaderOwner owner) { /*Noop*/ }

        /// <inheritdoc />
        public IEnumerable<IParameterComponentLifeCycle> ReadParameters() => _parameters;

        /// <inheritdoc />
        public void Complete() { /*Noop*/ }
    }
}
