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
using MudBlazor.State.Comparer;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a collection of multiple <see cref="ParameterScopeContainer"/> instances combined into a union.
/// </summary>
/// <remarks>
/// This class allows combining multiple <see cref="ParameterScopeContainer"/> instances into a single union, enabling the management of parameters across different scope containers.
/// </remarks>
internal class ParameterContainer : IParameterContainer
{
    private readonly Lazy<bool> _lazyVerify;
    private readonly List<IParameterScopeContainer> _parameterScopeContainers = new();

    /// <summary>
    /// Gets or sets a value indicating whether the container should automatically verify for duplicates.
    /// </summary>
    public bool AutoVerify { get; init; } = true;

    /// <summary>
    /// Gets the number of <see cref="ParameterScopeContainer"/> instances in the union.
    /// </summary>
    public int Count => _parameterScopeContainers.Count;

    /// <summary>
    /// Adds a <see cref="ParameterScopeContainer"/> instance to the union container.
    /// </summary>
    /// <param name="parameterScopeContainer">The <see cref="ParameterScopeContainer"/> instance to add to the union.</param>
    public void Add(IParameterScopeContainer parameterScopeContainer) => _parameterScopeContainers.Add(parameterScopeContainer);

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterContainer"/> class.
    /// </summary>
    public ParameterContainer()
    {
        _lazyVerify = new Lazy<bool>(VerifyInternal);
    }

    /// <summary>
    /// Executes <see cref="ParameterScopeContainer.OnInitialized"/> for all registered <see cref="ParameterScopeContainer"/>.
    /// </summary>
    public void OnInitialized()
    {
        VerifyOnAuto();

        foreach (var parameterSet in _parameterScopeContainers)
        {
            parameterSet.OnInitialized();
        }
    }

    /// <summary>
    /// Executes <see cref="ParameterScopeContainer.OnParametersSet"/> for all registered <see cref="ParameterScopeContainer"/>.
    /// </summary>
    public void OnParametersSet()
    {
        VerifyOnAuto();

        foreach (var parameterSet in _parameterScopeContainers)
        {
            parameterSet.OnParametersSet();
        }
    }

    /// <summary>
    /// Determines which <see cref="ParameterState{T}"/> have been changed and calls their respective change handler.
    /// </summary>
    /// <param name="baseSetParametersAsync">A func to call the base class' <see cref="ComponentBase.SetParametersAsync"/>.</param>
    /// <param name="parameters">The ParameterView coming from Blazor's  <see cref="ComponentBase.SetParametersAsync"/>.</param>
    public async Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters)
    {
        if (Count == 0)
        {
            await baseSetParametersAsync(parameters);
            return;
        }

        VerifyOnAuto();

#if NET8_0_OR_GREATER
        var parametersHandlerShouldFire = _parameterScopeContainers.SelectMany(parameter => parameter)
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToFrozenSet(ParameterHandlerUniquenessComparer.Default);
#else
        var parametersHandlerShouldFire = _parameterScopeContainers.SelectMany(parameter => parameter)
            .Where(parameter => parameter.HasHandler && parameter.HasParameterChanged(parameters))
            .ToHashSet(ParameterHandlerUniquenessComparer.Default);
#endif

        await baseSetParametersAsync(parameters);

        foreach (var parameterHandlerShouldFire in parametersHandlerShouldFire)
        {
            await parameterHandlerShouldFire.ParameterChangeHandleAsync();
        }
    }

    /// <inheritdoc/>
    public bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle)
    {
        VerifyOnAuto();

        foreach (var parameterSet in _parameterScopeContainers)
        {
            if (parameterSet.TryGetValue(parameterName, out parameterComponentLifeCycle))
            {
                return true;
            }
        }

        parameterComponentLifeCycle = null;

        return false;
    }

    /// <summary>
    /// Verifies the container for any duplicate parameters.
    /// </summary>
    public void Verify() => _ = _lazyVerify.Value;

    /// <summary>
    /// Throws an exception if <see cref="AutoVerify"/> is enabled and duplicates are found.
    /// </summary>
    private void VerifyOnAuto()
    {
        if (AutoVerify)
        {
            Verify();
        }
    }

    private bool VerifyInternal()
    {
        ThrowOnDuplicates();

        return true;
    }

    /// <summary>
    /// Throws an exception if duplicates are found among the parameter scope containers.
    /// </summary>
    private void ThrowOnDuplicates()
    {
        var hashSet = new HashSet<IParameterComponentLifeCycle>(ParameterNameUniquenessComparer.Default);
        var parameters = _parameterScopeContainers.SelectMany(scopeContainers => scopeContainers);

        foreach (var parameter in parameters)
        {
            if (!hashSet.Add(parameter))
            {
                throw new InvalidOperationException($"Parameter {parameter.Metadata.ParameterName} is already registered!");
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerator<IParameterComponentLifeCycle> GetEnumerator() => _parameterScopeContainers.SelectMany(scopeContainer => scopeContainer).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
