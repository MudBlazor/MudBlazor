// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a non-generic snapshot of a parameter's name, current value, and last value.
/// </summary>
/// <remarks>
/// This struct is used to pass parameter state information to shared change handlers
/// that need to coordinate changes across multiple parameters.
/// </remarks>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct ParameterStateValue
{
    /// <summary>
    /// Gets the name of the parameter.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the current value of the parameter.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Gets the last value of the parameter before the change.
    /// </summary>
    public object? LastValue { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterStateValue"/> struct.
    /// </summary>
    /// <param name="name">The name of the parameter.</param>
    /// <param name="lastValue">The last value of the parameter.</param>
    /// <param name="value">The current value of the parameter.</param>
    internal ParameterStateValue(string name, object? lastValue, object? value)
    {
        Name = name;
        LastValue = lastValue;
        Value = value;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Name}: {LastValue ?? "null"} -> {Value ?? "null"}";
}

/// <summary>
/// Debugger type proxy for <see cref="ParameterStateCollection"/> that provides a better view of the collection in the debugger.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class ParameterStateCollectionDebugView
{
    private readonly ParameterStateCollection _collection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterStateCollectionDebugView"/> class.
    /// </summary>
    /// <param name="collection">The collection to provide a debug view for.</param>
    public ParameterStateCollectionDebugView(ParameterStateCollection collection)
    {
        _collection = collection;
    }

    /// <summary>
    /// Gets an array of parameter state values for display in the debugger.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public ParameterStateValue[] Items
    {
        get
        {
            if (_collection.Count == 0)
            {
                return [];
            }

            var items = new ParameterStateValue[_collection.Count];
            var index = 0;
            var dictionary = _collection.Dictionary;
            if (dictionary is not null)
            {
                foreach (var kvp in dictionary)
                {
                    items[index++] = kvp.Value;
                }
            }

            return items;
        }
    }
}
