// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace MudBlazor.State.SmartBuilder;

#nullable enable
internal class SmartParameterBuilder<T>
{
    private string? _parameterName;
    private string? _handlerName;
    private Func<T>? _getParameterValueFunc;
    private Func<EventCallback<T>> _eventCallbackFunc = () => default;
    private IParameterChangedHandler<T>? _parameterChangedHandler;
    private Func<IEqualityComparer<T>?>? _comparerFunc;
    private readonly ISmartParameterSetRegister _smartParameterSetRegister;

    public SmartParameterBuilder(ISmartParameterSetRegister smartParameterSetRegister)
    {
        _smartParameterSetRegister = smartParameterSetRegister;
    }

    /// <summary>
    /// Sets the metadata for the parameter.
    /// </summary>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <returns>The current instance of the builder.</returns>
    public SmartParameterBuilder<T> WithParameterName(string parameterName)
    {
        _parameterName = parameterName;

        return this;
    }

    /// <summary>
    /// Sets the function to get the parameter value.
    /// </summary>
    /// <param name="getParameterValueFunc">The function to get the parameter value.</param>
    /// <returns>The current instance of the builder.</returns>
    public SmartParameterBuilder<T> WithGetParameterValueFunc(Func<T> getParameterValueFunc)
    {
        _getParameterValueFunc = getParameterValueFunc;

        return this;
    }

    /// <summary>
    /// Sets the function to create the event callback for the parameter.
    /// </summary>
    /// <param name="eventCallbackFunc">The function to create the event callback.</param>
    /// <returns>The current instance of the builder.</returns>
    public SmartParameterBuilder<T> WithEventCallbackFunc(Func<EventCallback<T>> eventCallbackFunc)
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
    public SmartParameterBuilder<T> WithParameterChangedHandler(IParameterChangedHandler<T> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public SmartParameterBuilder<T> WithParameterChangedHandler(Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public SmartParameterBuilder<T> WithParameterChangedHandler(Func<Task> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public SmartParameterBuilder<T> WithParameterChangedHandler(Action parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public SmartParameterBuilder<T> WithParameterChangedHandler(Action<ParameterChangedEventArgs<T>> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public SmartParameterBuilder<T> WithComparer(IEqualityComparer<T>? comparer)
    {
        _comparerFunc = () => comparer;

        return this;
    }

    /// <summary>
    /// Sets the comparer for the parameter.
    /// </summary>
    /// <param name="comparerFunc">The comparer for the parameter.</param>
    /// <returns>The current instance of the builder.</returns>
    public SmartParameterBuilder<T> WithComparer(Func<IEqualityComparer<T>>? comparerFunc)
    {
        _comparerFunc = comparerFunc;

        return this;
    }

    public ParameterState<T> Build()
    {
        ArgumentNullException.ThrowIfNull(_parameterName);

        var parameterState = ParameterState<T>.Attach(
            new ParameterMetadata(_parameterName, _handlerName),
            _getParameterValueFunc ?? throw new ArgumentNullException(nameof(_getParameterValueFunc)),
            _eventCallbackFunc,
            _parameterChangedHandler,
            _comparerFunc);

        _smartParameterSetRegister.Add(parameterState);

        return parameterState;
    }

    public static implicit operator ParameterState<T>(SmartParameterBuilder<T> builder) => builder.Build();
}
