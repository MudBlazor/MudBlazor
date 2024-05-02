// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Provides a deep copy resolver implementation for objects of type <typeparamref name="T"/> that implement the <see cref="ICloneable"/> interface.
/// </summary>
/// <typeparam name="T">The type of the object to be deep-copied, which must implement the <see cref="ICloneable"/> interface.</typeparam>
public sealed class CloneableDeepCopyResolver<T> : IDeepCopyResolver<T> where T : ICloneable
{
    /// <inheritdoc />
    public T? CloneObject(T item) => (T?)item.Clone();
}
