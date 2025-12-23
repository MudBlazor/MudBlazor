// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Frozen;
using MudBlazor.State.Comparer;
using MudBlazor.State.Invocation;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Utility class for handling parameter change detection and handler invocation.
/// </summary>
internal static class ParameterChangeHandlerUtility
{
    /// <summary>
    /// Represents a collection of handlers and their associated parameter changed context.
    /// </summary>
    internal readonly struct HandlerCollection
    {
        public List<IParameterStateInvocationSnapshot> Handlers { get; }
        public ParameterChangedContext Context { get; }

        public HandlerCollection(List<IParameterStateInvocationSnapshot> handlers, ParameterChangedContext context)
        {
            Handlers = handlers;
            Context = context;
        }
    }

    /// <summary>
    /// Adds a snapshot to the list if it's not a duplicate.
    /// Uses <see cref="ParameterHandlerUniquenessComparer"/> to check for duplicates.
    /// Also collects the parameter state value if available.
    /// </summary>
    /// <param name="snapshots">The list of snapshots to add to.</param>
    /// <param name="targetSnapshot">The snapshot to add if unique.</param>
    /// <param name="parameterStateValues">The list to collect parameter state values.</param>
    public static void AddSnapshotIfUnique(
        List<IParameterStateInvocationSnapshot> snapshots,
        IParameterStateInvocationSnapshot targetSnapshot,
        List<ParameterStateValue> parameterStateValues)
    {
        // Collect parameter state value if available (must happen before early return)
        var parameterStateValue = targetSnapshot.GetParameterStateValue();
        if (parameterStateValue.HasValue)
        {
            parameterStateValues.Add(parameterStateValue.Value);
        }

        // Check for duplicate handler and return early if found
        foreach (var snapshot in snapshots)
        {
            if (ParameterHandlerUniquenessComparer.Default.Equals(snapshot, targetSnapshot))
            {
                return;
            }
        }

        snapshots.Add(targetSnapshot);
    }

    /// <summary>
    /// Creates a handler collection with parameter changed context from the provided handlers.
    /// </summary>
    /// <param name="handlers">The list of handlers, or null if no handlers.</param>
    /// <param name="parameterStateValues">The list of collected parameter state values.</param>
    /// <param name="parameterView">The parameter view snapshot.</param>
    /// <returns>A <see cref="HandlerCollection"/> or null if no handlers.</returns>
    public static HandlerCollection? CreateHandlerCollection(
        List<IParameterStateInvocationSnapshot>? handlers,
        List<ParameterStateValue>? parameterStateValues,
        Microsoft.AspNetCore.Components.ParameterView parameterView)
    {
        if (handlers is null)
        {
            return null;
        }

        ParameterStateCollection parameterStates;
        if (parameterStateValues is not null && parameterStateValues.Count > 0)
        {
            // Create a frozen dictionary for O(1) lookup performance
            var dictionary = parameterStateValues.ToFrozenDictionary(
                p => p.Name,
                p => p,
                StringComparer.Ordinal);
            parameterStates = new ParameterStateCollection(dictionary);
        }
        else
        {
            parameterStates = ParameterStateCollection.Empty;
        }

        var context = new ParameterChangedContext(parameterView, parameterStates);

        return new HandlerCollection(handlers, context);
    }

    /// <summary>
    /// Invokes all handlers in the provided collection asynchronously.
    /// </summary>
    /// <param name="handlerCollection">The handler collection to invoke, or null if no handlers.</param>
    public static async Task InvokeHandlersAsync(HandlerCollection? handlerCollection)
    {
        if (handlerCollection.HasValue)
        {
            var collection = handlerCollection.Value;
            foreach (var handler in collection.Handlers)
            {
                await handler.ParameterChangeHandleAsync(collection.Context);
            }
        }
    }
}

