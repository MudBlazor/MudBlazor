// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Helper class for creating <see cref="ParameterState{T}"/> object with different overloads to initialize <see cref="ParameterState{T}"/>.
/// </summary>
[ExcludeFromCodeCoverage]
internal class ParameterState
{
    #region (ParameterMetadata, ParameterValue, EventCallback, Action)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Action parameterChangedHandler)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaHandler<T>(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Action<ParameterChangedEventArgs<T>> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaArgsHandler<T>(parameterChangedHandler));
    }

    #endregion

    #region (ParameterMetadata, ParameterValue, EventCallback, Func)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Func<Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaTaskHandler<T>(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaArgsTaskHandler<T>(parameterChangedHandler));
    }

    #endregion

    #region (ParameterMetadata, ParameterValue, EventCallback)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, eventCallbackFunc);
    }

    #endregion

    #region (ParameterMetadata, ParameterValue, Action)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Action parameterChangedHandler)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, () => default, new ParameterChangedLambdaHandler<T>(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Action<ParameterChangedEventArgs<T>> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, () => default, new ParameterChangedLambdaArgsHandler<T>(parameterChangedHandler));
    }

    #endregion

    #region (ParameterMetadata, ParameterValue, Func)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, () => default, new ParameterChangedLambdaTaskHandler<T>(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="metadata">The parameter's metadata.</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(ParameterMetadata metadata, Func<T> getParameterValueFunc, Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(metadata, getParameterValueFunc, () => default, new ParameterChangedLambdaArgsTaskHandler<T>(parameterChangedHandler));
    }

    #endregion

    #region (ParameterName, ParameterValue, EventCallback, Action)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Action parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaHandler<T>(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Action<ParameterChangedEventArgs<T>> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaArgsHandler<T>(parameterChangedHandler));
    }

    #endregion

    #region (ParameterName, ParameterValue, EventCallback, Func)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Func<Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaTaskHandler<T>(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc, new ParameterChangedLambdaArgsTaskHandler<T>(parameterChangedHandler));
    }

    #endregion

    #region (ParameterName, ParameterValue, EventCallback)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, eventCallbackFunc);
    }

    #endregion

    #region (ParameterName, ParameterValue, Action)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Action parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, () => default, new ParameterChangedLambdaHandler<T>(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Action<ParameterChangedEventArgs<T>> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, () => default, new ParameterChangedLambdaArgsHandler<T>(parameterChangedHandler));
    }

    #endregion

    #region (ParameterName, ParameterValue, Func)

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, () => default, new ParameterChangedLambdaTaskHandler<T>(parameterChangedHandler));
    }

    /// <summary>
    /// Creates a <see cref="ParameterState{T}"/> object which automatically manages parameter value changes as part of MudBlazor's ParameterState framework.
    /// </summary>
    /// <typeparam name="T">The type of the component's property value.</typeparam>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <param name="getParameterValueFunc">A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <remarks>
    /// For details and usage, please read CONTRIBUTING.md
    /// </remarks>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    public static ParameterState<T> Attach<T>(string parameterName, Func<T> getParameterValueFunc, Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler)
    {
        return ParameterState<T>.Attach(parameterName, getParameterValueFunc, () => default, new ParameterChangedLambdaArgsTaskHandler<T>(parameterChangedHandler));
    }

    #endregion
}
