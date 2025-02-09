// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.Utilities.Clone;

#nullable enable
/// <summary>
/// Provides a deep copy implementation for objects of type <typeparamref name="T"/> that implement the <see cref="ICloneable"/> interface.
/// </summary>
/// <typeparam name="T">The type of the object to be deep-copied, which must implement the <see cref="ICloneable"/> interface.</typeparam>
public sealed class CloneableCloneStrategy<T> : ICloneStrategy<T> where T : ICloneable
{
    /// <inheritdoc />
    public T? CloneObject(T item) => (T?)item.Clone();

    /// <summary>
    /// Represents a static field providing an instance of <see cref="CloneableCloneStrategy{T}"/>.
    /// </summary>
    public static readonly ICloneStrategy<T> Instance = new CloneableCloneStrategy<T>();
}
