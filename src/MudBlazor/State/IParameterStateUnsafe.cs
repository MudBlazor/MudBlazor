// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents an unsafe parameter state that allows getting or setting the parameter's value.
/// </summary>
/// <typeparam name="T">The type of the parameter.</typeparam>
internal interface IParameterStateUnsafe<T>
{
    /// <summary>
    /// Gets or sets the current value.
    /// </summary>
    T? Value { get; set; }

    /// <summary>
    /// Gets the callback that is invoked when the parameter's value changes.
    /// </summary>
    EventCallback<T> ValueChanged { get; }

    /// <summary>
    /// Gets the comparer for the parameter.
    /// </summary>
    IEqualityComparer<T> Comparer { get; }
}
