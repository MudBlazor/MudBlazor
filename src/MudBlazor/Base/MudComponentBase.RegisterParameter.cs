using System;
using System.Collections.Generic;
using System.Linq;
using MudBlazor.State;
using MudBlazor.State.Builder;

namespace MudBlazor;

#nullable enable
public abstract partial class MudComponentBase : IParameterStatesFactoryReader, IParameterStatesFactoryWriter
{
    private readonly ParameterRegistrationBuilderScope _scope;
    private Func<IEnumerable<IParameterComponentLifeCycle>>? _componentLifeCycleFactory;

    /// <summary>
    /// Creates a scope for registering parameters.
    /// </summary>
    /// <returns>A <see cref="ParameterRegistrationBuilderScope"/> instance for registering parameters.</returns>
    internal IParameterRegistrationBuilderScope CreateRegisterScope()
    {
        if (_scope.IsLocked)
        {
            throw new InvalidOperationException($"You are not allowed to create more than one {nameof(CreateRegisterScope)}!");
        }

        return _scope;
    }

    /// <inheritdoc />
    IEnumerable<IParameterComponentLifeCycle> IParameterStatesFactoryReader.ReadParameters()
    {
        return _componentLifeCycleFactory is null
            ? Enumerable.Empty<IParameterComponentLifeCycle>()
            : _componentLifeCycleFactory();
    }

    /// <inheritdoc />
    void IParameterStatesFactoryReader.Complete() => _scope.CleanUp();

    /// <inheritdoc />
    void IParameterStatesFactoryWriter.WriteParameters(IEnumerable<IParameterComponentLifeCycle> parameters) => _componentLifeCycleFactory = () => parameters;

    /// <inheritdoc />
    void IParameterStatesFactoryWriter.Close() => Parameters.ForceParametersAttachment();
}
