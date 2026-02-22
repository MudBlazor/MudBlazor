// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor.State.Invocation;


/// <summary>
/// Implements <see cref="IParameterStateInvocationSnapshot"/>.
/// </summary>
/// <typeparam name="T">The type of the component's property value.</typeparam>
internal class ParameterStateInvocationSnapshot<T> : IParameterStateInvocationSnapshot
{
    private readonly Func<bool> _isChildOriginatedChangeFunc;
    private readonly ParameterChangedEventArgs<T>? _parameterChangedEventArgs;
    private readonly IParameterChangedHandler<T>? _parameterChangedHandler;

    /// <inheritdoc />
    public ParameterMetadata Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterStateInvocationSnapshot{T}"/> class.
    /// </summary>
    /// <param name="parameterMetadata">Metadata describing the parameter.</param>
    /// <param name="parameterChangedEventArgs">A cloned instance of the last parameter change event arguments, if any.</param>
    /// <param name="parameterChangedHandler">The registered handler responsible for processing parameter changes.</param>
    /// <param name="isChildOriginatedChangeFunc">Indicates whether the change originated from a child component.</param>
    /// <remarks>
    /// This constructor is intentionally internal. Snapshots are created exclusively through
    /// <see cref="ParameterStateInternal{T}.CreateInvocationSnapshot"/> to ensure lifecycle consistency.
    /// </remarks>
    public ParameterStateInvocationSnapshot(ParameterMetadata parameterMetadata, ParameterChangedEventArgs<T>? parameterChangedEventArgs, IParameterChangedHandler<T>? parameterChangedHandler, Func<bool> isChildOriginatedChangeFunc)
    {
        Metadata = parameterMetadata;
        _parameterChangedEventArgs = parameterChangedEventArgs;
        _parameterChangedHandler = parameterChangedHandler;
        _isChildOriginatedChangeFunc = isChildOriginatedChangeFunc;
    }

    [MemberNotNullWhen(true, nameof(_parameterChangedEventArgs))]
    private bool HasParameterChangedEventArgs => _parameterChangedEventArgs is not null;

    [MemberNotNullWhen(true, nameof(_parameterChangedHandler))]
    private bool HasHandler => _parameterChangedHandler is not null;

    /// <inheritdoc />
    public Task ParameterChangeHandleAsync(ParameterChangedContext context)
    {
        if (HasHandler && HasParameterChangedEventArgs)
        {
            // Since the ParameterSet lifecycles control all operations, it is acceptable to trigger the handler only when
            // HasParameterChanged has been invoked and stored the ParameterChangedEventArgs.
            // Direct invocation of this method by external callers is discouraged, so we shouldn't worry about it.
            return _parameterChangedHandler.HandleAsync(_parameterChangedEventArgs.ChildOriginated(_isChildOriginatedChangeFunc()), context);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public ParameterStateValue? GetParameterStateValue()
    {
        if (HasParameterChangedEventArgs)
        {
            return new ParameterStateValue(
                Metadata.ParameterName,
                _parameterChangedEventArgs.LastValue,
                _parameterChangedEventArgs.Value);
        }

        return null;
    }
}
