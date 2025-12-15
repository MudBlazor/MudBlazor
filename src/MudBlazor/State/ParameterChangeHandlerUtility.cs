// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    /// </summary>
    /// <param name="snapshots">The list of snapshots to add to.</param>
    /// <param name="targetSnapshot">The snapshot to add if unique.</param>
    public static void AddSnapshotIfUnique(List<IParameterStateInvocationSnapshot> snapshots, IParameterStateInvocationSnapshot targetSnapshot)
    {
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
    /// Invokes all handlers in the provided list asynchronously.
    /// </summary>
    /// <param name="handlers">The list of handlers to invoke, or null if no handlers.</param>
    public static async Task InvokeHandlersAsync(List<IParameterStateInvocationSnapshot>? handlers)
    {
        if (handlers is not null)
        {
            foreach (var handler in handlers)
            {
                await handler.ParameterChangeHandleAsync();
            }
        }
    }
}
