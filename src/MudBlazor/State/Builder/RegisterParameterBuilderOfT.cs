// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Builder class for constructing instances of <see cref="ParameterState{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the component's property value.</typeparam>
internal class RegisterParameterBuilder<T>
{
    private string? _parameterName;
    private string? _handlerName;
    private Func<T>? _getParameterValueFunc;
    private Func<EventCallback<T>> _eventCallbackFunc = () => default;
    private IParameterChangedHandler<T>? _parameterChangedHandler;
    private Func<IEqualityComparer<T>?>? _comparerFunc;
    private readonly IParameterSetRegister _smartParameterSetRegister;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterParameterBuilder{T}"/> class.
    /// </summary>
    /// <param name="smartParameterSetRegister">The <see cref="IParameterSetRegister"/> used to register the parameter during the <see cref="Build"/>.</param>
    public RegisterParameterBuilder(IParameterSetRegister smartParameterSetRegister)
    {
        _smartParameterSetRegister = smartParameterSetRegister;
    }

    /// <summary>
    /// Sets the metadata for the parameter.
    /// </summary>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithParameterName(string parameterName)
    {
        _parameterName = parameterName;

        return this;
    }

    /// <summary>
    /// Sets the function to get the parameter value.
    /// </summary>
    /// <param name="getParameterValueFunc">The function to get the parameter value.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithGetParameterValueFunc(Func<T> getParameterValueFunc)
    {
        _getParameterValueFunc = getParameterValueFunc;

        return this;
    }

    /// <summary>
    /// Sets the function to create the event callback for the parameter.
    /// </summary>
    /// <param name="eventCallbackFunc">The function to create the event callback.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithEventCallbackFunc(Func<EventCallback<T>> eventCallbackFunc)
    {
        _eventCallbackFunc = eventCallbackFunc;

        return this;
    }

    /// <summary>
    /// Sets the parameter changed handler for the parameter.
    /// </summary>
    /// <param name="parameterChangedHandler">The parameter changed handler.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithParameterChangedHandler(IParameterChangedHandler<T> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        _parameterChangedHandler = parameterChangedHandler;
        _handlerName = handlerName;

        return this;
    }

    /// <summary>
    /// Sets the parameter changed handler for the parameter.
    /// </summary>
    /// <param name="parameterChangedHandler">The parameter changed handler.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithParameterChangedHandler(Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        _parameterChangedHandler = new ParameterChangedLambdaArgsTaskHandler<T>(parameterChangedHandler);
        _handlerName = handlerName;

        return this;
    }

    /// <summary>
    /// Sets the parameter changed handler for the parameter.
    /// </summary>
    /// <param name="parameterChangedHandler">The parameter changed handler.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithParameterChangedHandler(Func<Task> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        _parameterChangedHandler = new ParameterChangedLambdaTaskHandler<T>(parameterChangedHandler);
        _handlerName = handlerName;

        return this;
    }

    /// <summary>
    /// Sets the parameter changed handler for the parameter.
    /// </summary>
    /// <param name="parameterChangedHandler">The parameter changed handler.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithParameterChangedHandler(Action parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        _parameterChangedHandler = new ParameterChangedLambdaHandler<T>(parameterChangedHandler);
        _handlerName = handlerName;

        return this;
    }

    /// <summary>
    /// Sets the parameter changed handler for the parameter.
    /// </summary>
    /// <param name="parameterChangedHandler">The parameter changed handler.</param>
    /// <param name="handlerName">The handler's name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithParameterChangedHandler(Action<ParameterChangedEventArgs<T>> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        _parameterChangedHandler = new ParameterChangedLambdaArgsHandler<T>(parameterChangedHandler);
        _handlerName = handlerName;

        return this;
    }

    /// <summary>
    /// Sets the comparer for the parameter.
    /// </summary>
    /// <param name="comparer">The comparer for the parameter.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithComparer(IEqualityComparer<T>? comparer)
    {
        _comparerFunc = () => comparer;

        return this;
    }

    /// <summary>
    /// Sets the function to provide the comparer for the parameter.
    /// </summary>
    /// <param name="comparerFunc">The function to provide the comparer for the parameter.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithComparer(Func<IEqualityComparer<T>>? comparerFunc)
    {
        _comparerFunc = comparerFunc;

        return this;
    }

    /// <summary>
    /// Builds and registers the parameter state to <see cref="ParameterSet"/>.
    /// </summary>
    /// <returns>The created parameter state.</returns>
    public ParameterStateInternal<T> Build()
    {
        ArgumentNullException.ThrowIfNull(_parameterName);

        var parameterState = ParameterStateInternal<T>.Attach(
            new ParameterMetadata(_parameterName, _handlerName),
            _getParameterValueFunc ?? throw new ArgumentNullException(nameof(_getParameterValueFunc)),
            _eventCallbackFunc,
            _parameterChangedHandler,
            _comparerFunc);

        _smartParameterSetRegister.Add(parameterState);

        return parameterState;
    }

    /// <summary>
    /// Implicitly converts a <see cref="RegisterParameterBuilder{T}"/> object to a <see cref="ParameterState{T}"/> object by building it.
    /// </summary>
    /// <param name="builder">The <see cref="RegisterParameterBuilder{T}"/> object to convert.</param>
    /// <returns>The created <see cref="ParameterState{T}"/> object.</returns>
    public static implicit operator ParameterState<T>(RegisterParameterBuilder<T> builder) => builder.Build();
}
