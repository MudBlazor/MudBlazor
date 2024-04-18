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
/// Represents a hot swappable parameter comparer.
/// </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
[DebuggerDisplay("IEqualityComparer = {OriginalComparer}")]
internal class ParameterEqualityComparerSwappable<T> : IParameterEqualityComparerSwappable<T>
{
    /// <inheritdoc />
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public Func<IEqualityComparer<T>> UnderlyingComparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterEqualityComparerSwappable{T}"/> class with the specified comparer function.
    /// </summary>
    /// <remarks>
    /// This constructor should be used when you have a static <see cref="IEqualityComparer{T}"/> that does not change during the lifetime of a component.
    /// </remarks>
    /// <param name="comparer">The static comparer for the parameter.</param>
    public ParameterEqualityComparerSwappable(IEqualityComparer<T>? comparer)
        : this(() => comparer ?? EqualityComparer<T>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterEqualityComparerSwappable{T}"/> class with the specified comparer function.
    /// </summary>
    /// <param name="comparerFunc">The function to provide the comparer for the parameter.</param>
    public ParameterEqualityComparerSwappable(Func<IEqualityComparer<T>>? comparerFunc)
    {
        UnderlyingComparer = comparerFunc ?? (() => EqualityComparer<T>.Default);
    }

    /// <inheritdoc />
    public bool Equals(T? x, T? y) => UnderlyingComparer().Equals(x, y);

    /// <inheritdoc />
    public int GetHashCode([DisallowNull] T obj) => UnderlyingComparer().GetHashCode(obj);

    /// <inheritdoc />
    public bool TryGetFromParameterView(ParameterView parameters, string parameterName, [MaybeNullWhen(false)] out IEqualityComparer<T> result) => parameters.TryGetValue(parameterName, out result);

    [ExcludeFromCodeCoverage]
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private IEqualityComparer<T> OriginalComparer => UnderlyingComparer();
}
