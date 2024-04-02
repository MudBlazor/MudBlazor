using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.State.Builder;

namespace MudBlazor;

#nullable enable
public abstract partial class MudComponentBase
{
    #region (ParameterName, ParameterValue, EventCallback, Action, IEqualityComparer)

    /// <summary>
    /// Register a component Parameter, its EventCallback and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Action parameterChangedHandler, IEqualityComparer<T>? comparer = null, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName, handlerName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithEventCallbackFunc(eventCallbackFunc)
            .WithParameterChangedHandler(parameterChangedHandler)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }
    /// <summary>
    /// Register a component Parameter, its EventCallback and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Action<ParameterChangedEventArgs<T>> parameterChangedHandler, IEqualityComparer<T>? comparer = null, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName, handlerName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithEventCallbackFunc(eventCallbackFunc)
            .WithParameterChangedHandler(parameterChangedHandler)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }

    #endregion

    #region (ParameterName, ParameterValue, EventCallbck, Func, IEqualityComparer)

    /// <summary>
    /// Register a component Parameter, its EventCallback and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Func<Task> parameterChangedHandler, IEqualityComparer<T>? comparer = null, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName, handlerName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithEventCallbackFunc(eventCallbackFunc)
            .WithParameterChangedHandler(parameterChangedHandler)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }

    /// <summary>
    /// Register a component Parameter, its EventCallback and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler, IEqualityComparer<T>? comparer = null, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName, handlerName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithEventCallbackFunc(eventCallbackFunc)
            .WithParameterChangedHandler(parameterChangedHandler)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }

    #endregion

    #region (ParameterName, ParameterValue, EventCallback, IEqualityComparer)

    /// <summary>
    /// Register a component Parameter, its EventCallback and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="eventCallbackFunc">A function that allows <see cref="ParameterState{T}"/> to get the <see cref="EventCallback{T}"/> of the parameter.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Func<EventCallback<T>> eventCallbackFunc, IEqualityComparer<T>? comparer = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithEventCallbackFunc(eventCallbackFunc)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }

    #endregion

    #region (ParameterName, ParameterValue, Action, IEqualityComparer)

    /// <summary>
    /// Register a component Parameter and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Action parameterChangedHandler, IEqualityComparer<T>? comparer = null, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName, handlerName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithParameterChangedHandler(parameterChangedHandler)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }

    /// <summary>
    /// Register a component Parameter and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">An action containing code that needs to be executed when the parameter value changes.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Action<ParameterChangedEventArgs<T>> parameterChangedHandler, IEqualityComparer<T>? comparer = null, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName, handlerName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithParameterChangedHandler(parameterChangedHandler)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }

    #endregion

    #region (ParameterName, ParameterValue, Func, IEqualityComparer)

    /// <summary>
    /// Register a component Parameter and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Func<Task> parameterChangedHandler, IEqualityComparer<T>? comparer = null, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName, handlerName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithParameterChangedHandler(parameterChangedHandler)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }

    /// <summary>
    /// Register a component Parameter and a change handler so that the base can manage it as a ParameterState object.
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
    /// <param name="getParameterValueFunc">>A function that allows <see cref="ParameterState{T}"/> to read the property value.</param>
    /// <param name="parameterChangedHandler">A function containing code that needs to be executed when the parameter value changes.</param>
    /// <param name="comparer">An optional comparer used to determine equality of parameter values.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The <see cref="ParameterState{T}"/> object to be stored in a field for accessing the current state value.</returns>
    internal IParameterState<T> RegisterParameter<T>(string parameterName, Func<T> getParameterValueFunc, Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler, IEqualityComparer<T>? comparer = null, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        var parameterState = ParameterAttachBuilder
            .Create<T>()
            .WithMetadata(new ParameterMetadata(parameterName, handlerName))
            .WithGetParameterValueFunc(getParameterValueFunc)
            .WithParameterChangedHandler(parameterChangedHandler)
            .WithComparer(comparer)
            .Attach();
        _parameters.Add(parameterState);

        return parameterState;
    }

    #endregion
}
