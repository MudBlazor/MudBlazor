using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// The <see cref="ParameterState{T}"/> automatically manages parameter value changes for <see cref="ParameterAttribute"/> as part of
/// MudBlazor's ParameterState framework. For details and usage please read CONTRIBUTING.md
/// </summary>
/// <remarks>
/// You don't need to create this object directly.
/// Instead, use the "MudComponentBase.RegisterParameter" method from within the component's constructor.
/// </remarks>
/// <typeparam name="T">The type of the component's property value.</typeparam>
internal interface IParameterState<T>
{
    /// <summary>
    /// Gets the current value.
    /// </summary>
    T? Value { get; }

    /// <summary>
    /// Set the parameter's value. 
    /// </summary>
    /// <remarks>
    /// Note: you should never set the parameter's property directly from within the component.
    /// Instead, use SetValueAsync on the ParameterState object.
    /// </remarks>
    /// <param name="value">New parameter's value.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SetValueAsync(T value);
}
