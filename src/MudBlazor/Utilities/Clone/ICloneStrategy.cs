// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Clone;

#nullable enable
/// <summary>
/// Represents an interface for resolving deep copy operations for objects of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// Please ensure that it implements deep copy logic for all nested objects, not just shallow copies.
/// </remarks>
/// <typeparam name="T">The type of the object to be deep-copied.</typeparam>
public interface ICloneStrategy<T>
{
    /// <summary>
    /// Clones the specified object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="item">The object to clone.</param>
    /// <returns>A deep copy of the object.</returns>
    T? CloneObject(T item);
}
