using System.Collections.Concurrent;
using MudBlazor.State;
using MudBlazor.State.Builder;

namespace MudBlazor;

#nullable enable
public abstract partial class MudComponentBase : IParameterSetRegister
{
    private bool _attachedAll;
    private readonly ConcurrentQueue<ISmartAttachable> _smartAttachables = new();

    /// <inheritdoc />
    void IParameterSetRegister.Add<T>(ParameterStateInternal<T> parameterState) => Parameters.Add(parameterState);

    /// <inheritdoc />
    void IParameterSetRegister.Add(ISmartAttachable smartAttachable) => _smartAttachables.Enqueue(smartAttachable);

    private void AttachAllUnAttached()
    {
        if (_attachedAll)
        {
            return;
        }

        try
        {
            while (_smartAttachables.TryDequeue(out var smartAttachable))
            {
                if (!smartAttachable.IsAttached)
                {
                    smartAttachable.Attach();
                }
            }
        }
        finally
        {
            _attachedAll = true;
        }
    }

    /// <summary>
    /// Register a component Parameter, its EventCallback and a change handler via a builder so that the base can manage it as a ParameterState object.
    /// It is the new rule in MudBlazor, that parameters must be auto properties.
    /// By registering the parameter with a change handler you can still execute code when the parameter value changes.
    /// This class is part of MudBlazor's ParameterState framework.
    /// <para />
    /// <b>NB!</b> This method must be called in the constructor!
    /// </summary>
    /// <remarks>
    /// See CONTRIBUTING.md for a more detailed explanation on why MudBlazor parameters have to registered. 
    /// </remarks>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <returns>A new instance of <see cref="RegisterParameterBuilder{T}"/>.</returns>
    internal RegisterParameterBuilder<T> RegisterParameterBuilder<T>(string parameterName)
    {
        var parameterState = State.Builder.RegisterParameterBuilder
            .Create<T>(this)
            .WithName(parameterName);

        return parameterState;
    }
}
