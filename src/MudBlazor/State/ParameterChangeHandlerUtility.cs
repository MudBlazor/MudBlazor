// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
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
        // IMPORTANT:
        // The parameter state value must be collected *before* the duplicate snapshot check.
        // Multiple parameters can share the same handler, which means the snapshot may be
        // considered a duplicate and rejected below. However, the shared handler still needs
        // to see *all* changed parameters via ParameterStateCollection.
        // If we return early without collecting here, ParameterStateValues would be incomplete
        // for shared handlers.
        var parameterStateValue = targetSnapshot.GetParameterStateValue();
        if (parameterStateValue.HasValue)
        {
            parameterStateValues.Add(parameterStateValue.Value);
        }

        foreach (var snapshot in snapshots)
        {
            if (ParameterHandlerUniquenessComparer.Default.Equals(snapshot, targetSnapshot))
            {
                // Handler is a duplicate, but parameter state value was already collected above
                // to support shared handlers.
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
        IReadOnlyList<IParameterStateInvocationSnapshot>? handlers,
        IReadOnlyList<ParameterStateValue>? parameterStateValues,
        ParameterView parameterView)
    {
        if (handlers is null)
        {
            return null;
        }

        ParameterStateCollection parameterStates;
        if (parameterStateValues is { Count: > 0 })
        {
            var dictionary = new Dictionary<string, ParameterStateValue>(parameterStateValues.Count, StringComparer.Ordinal);
            foreach (var parameter in parameterStateValues)
            {
                dictionary[parameter.Name] = parameter;
            }
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
    public static Task InvokeHandlersAsync(HandlerCollection? handlerCollection)
    {
        if (handlerCollection is null)
        {
            return Task.CompletedTask;
        }

        return InvokeCore(handlerCollection);

        static async Task InvokeCore(HandlerCollection handlerCollection)
        {
            foreach (var handler in handlerCollection.Handlers)
            {
                await handler.ParameterChangeHandleAsync(handlerCollection.Context).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Represents a collection of handlers and their associated parameter changed context.
    /// </summary>
    internal sealed class HandlerCollection(IReadOnlyList<IParameterStateInvocationSnapshot> handlers, ParameterChangedContext context)
    {
        public ParameterChangedContext Context { get; } = context;

        public IReadOnlyList<IParameterStateInvocationSnapshot> Handlers { get; } = handlers;
    }
}

