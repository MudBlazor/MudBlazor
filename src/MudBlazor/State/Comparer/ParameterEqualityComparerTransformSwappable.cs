// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State.Comparer;

#nullable enable
/// <summary>
/// Represents a hot swappable parameter comparer that can transform from one type to another.
/// </summary>
/// <typeparam name="TFrom">The type of the comparer to transform from.</typeparam>
/// <typeparam name="T">The type of objects to compare.</typeparam>
[DebuggerDisplay("IEqualityComparer = {OriginalComparer}")]
internal class ParameterEqualityComparerTransformSwappable<TFrom, T> : IParameterEqualityComparerSwappable<T>
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Func<IEqualityComparer<TFrom>, IEqualityComparer<T>> _comparerToFunc;
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly Func<IEqualityComparer<TFrom>> _comparerFromFunc;

    /// <inheritdoc />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Func<IEqualityComparer<T>> UnderlyingComparer => () => _comparerToFunc(_comparerFromFunc());

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterEqualityComparerTransformSwappable{TFrom, T}"/> class with the specified comparer transformation functions.
    /// </summary>
    /// <param name="comparerFromFunc">The function to provide the original comparer.</param>
    /// <param name="comparerToFunc">The function to transform the original comparer to the target comparer.</param>
    public ParameterEqualityComparerTransformSwappable(Func<IEqualityComparer<TFrom>> comparerFromFunc, Func<IEqualityComparer<TFrom>, IEqualityComparer<T>> comparerToFunc)
    {
        _comparerFromFunc = comparerFromFunc;
        _comparerToFunc = comparerToFunc;
    }

    /// <inheritdoc />
    public bool Equals(T? x, T? y) => UnderlyingComparer().Equals(x, y);

    /// <inheritdoc />
    public int GetHashCode([DisallowNull] T obj) => UnderlyingComparer().GetHashCode(obj);

    /// <inheritdoc />
    public bool TryGetFromParameterView(ParameterView parameters, string parameterName, [MaybeNullWhen(false)] out IEqualityComparer<T> result)
    {
        if (parameters.TryGetValue<IEqualityComparer<TFrom>>(parameterName, out var newComparer))
        {
            var comparer = _comparerToFunc(newComparer);
            result = comparer;

            return true;
        }

        result = default;

        return false;
    }

    [ExcludeFromCodeCoverage]
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private IEqualityComparer<T> OriginalComparer => UnderlyingComparer();
}
