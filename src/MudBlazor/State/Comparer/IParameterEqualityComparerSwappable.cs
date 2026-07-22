// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State.Comparer;

#nullable enable
/// <summary>
/// Represents an interface for a hot swappable parameter comparer.
/// </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
internal interface IParameterEqualityComparerSwappable<T> : IEqualityComparer<T>
{
    /// <summary>
    /// Gets the original unwrapped comparer function.
    /// </summary>
    Func<IEqualityComparer<T>> UnderlyingComparer { get; }

    /// <summary>
    /// Tries to get the comparer from the specified parameter view.
    /// </summary>
    /// <param name="parameters">The parameter view to search for the comparer.</param>
    /// <param name="parameterName">The name of the parameter containing the comparer.</param>
    /// <param name="result">When this method returns, contains the comparer associated with the specified parameter name, if the parameter was found; otherwise, the default value for the type of the result parameter. This parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the parameter view contains the comparer with the specified name; otherwise, <see langword="false"/>.</returns>
    bool TryGetFromParameterView(ParameterView parameters, string parameterName, [MaybeNullWhen(false)] out IEqualityComparer<T> result);
}
