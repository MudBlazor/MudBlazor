// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.State.Invocation;

/// <summary>
/// Represents a read-only, immutable snapshot of a parameter's invocation state.
/// </summary>
/// <remarks>
/// <para>
/// This interface exists to decouple parameter change invocation logic from the live parameter state.
/// Instead of directly invoking <see cref="IParameterChangedHandler{T}"/> during a render or lifecycle update,
/// a snapshot is created containing all necessary data to safely execute the handler later.
/// </para>
/// <para>
/// This ensures that concurrent lifecycle operations, reentrant parameter updates,
/// or asynchronous component updates cannot interfere with a live state mutation.
/// </para>
/// </remarks>
internal interface IParameterStateInvocationSnapshot
{
    /// <summary>
    /// Gets metadata associated with the parameter, including its name, handler name etc.
    /// </summary>
    ParameterMetadata Metadata { get; }

    /// <summary>
    /// Called by the <see cref="ParameterState{T}"/> framework when <see cref="IParameterChangedHandler{T}"/> is supplied.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method is intended for internal use and is controlled by the <see cref="MudComponentBase"/> and <see cref="ParameterScopeContainer"/>.
    /// It should only be invoked after <see cref="IParameterComponentLifeCycle.HasParameterChanged"/> has been called.
    /// </para>
    /// <para>
    /// Direct invocation of this method from external code is discouraged,
    /// as it may bypass lifecycle consistency guarantees.
    /// </para>
    /// </remarks>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task ParameterChangeHandleAsync();
}
