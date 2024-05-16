// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.State.Comparer;

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Builder class for constructing instances of <see cref="ParameterState{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the component's property value.</typeparam>
public class RegisterParameterBuilder<T> : IParameterBuilderAttach
{
    private string? _handlerName;
    private string? _parameterName;
    private string? _comparerParameterName;
    private Func<T>? _getParameterValueFunc;
    private Func<EventCallback<T>> _eventCallbackFunc = () => default;
    private IParameterChangedHandler<T>? _parameterChangedHandler;
    private IParameterEqualityComparerSwappable<T>? _comparer;
    private readonly Lazy<ParameterStateInternal<T>> _parameterStateLazy;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterParameterBuilder{T}"/> class.
    /// </summary>
    public RegisterParameterBuilder()
    {
        _parameterStateLazy = new Lazy<ParameterStateInternal<T>>(CreateParameterState);
    }

    /// <summary>
    /// Sets the metadata for the parameter.
    /// </summary>
    /// <param name="parameterName">The name of the parameter, passed using nameof(...).</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithName(string parameterName)
    {
        _parameterName = parameterName;

        return this;
    }

    /// <summary>
    /// Sets the function to get the parameter value.
    /// </summary>
    /// <param name="getParameterValueFunc">The function to get the parameter value.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithParameter(Func<T> getParameterValueFunc)
    {
        _getParameterValueFunc = getParameterValueFunc;

        return this;
    }

    /// <summary>
    /// Sets the function to create the event callback for the parameter.
    /// </summary>
    /// <param name="eventCallbackFunc">The function to create the event callback.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithEventCallback(Func<EventCallback<T>> eventCallbackFunc)
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
    public RegisterParameterBuilder<T> WithChangeHandler(IParameterChangedHandler<T> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public RegisterParameterBuilder<T> WithChangeHandler(Func<ParameterChangedEventArgs<T>, Task> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public RegisterParameterBuilder<T> WithChangeHandler(Func<Task> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public RegisterParameterBuilder<T> WithChangeHandler(Action parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
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
    public RegisterParameterBuilder<T> WithChangeHandler(Action<ParameterChangedEventArgs<T>> parameterChangedHandler, [CallerArgumentExpression(nameof(parameterChangedHandler))] string? handlerName = null)
    {
        _parameterChangedHandler = new ParameterChangedLambdaArgsHandler<T>(parameterChangedHandler);
        _handlerName = handlerName;

        return this;
    }

    /// <summary>
    /// Sets the comparer for the parameter.
    /// </summary>
    /// <param name="comparer">The comparer for the parameter.</param>
    /// <remarks>This method is used when you need to register a static <see cref="IEqualityComparer{T}"/> that doesn't change during <see cref="ParameterState{T}"/> lifespan.</remarks>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithComparer(IEqualityComparer<T>? comparer)
    {
        _comparer = new ParameterEqualityComparerSwappable<T>(comparer);

        return this;
    }

    /// <summary>
    /// Sets the function to provide the comparer for the parameter.
    /// </summary>
    /// <param name="comparerFunc">The function to provide the comparer for the parameter.</param>
    /// <param name="comparerParameterName">The parameter's comparer name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <remarks>This method should be used exclusively when the parameter has an associated <see cref="IEqualityComparer{T}" /> that is also declared as a Blazor <see cref="ParameterAttribute"/>.</remarks>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithComparer(Func<IEqualityComparer<T>>? comparerFunc, [CallerArgumentExpression(nameof(comparerFunc))] string? comparerParameterName = null)
    {
        _comparer = new ParameterEqualityComparerSwappable<T>(comparerFunc);
        _comparerParameterName = comparerParameterName;

        return this;
    }

    /// <summary>
    /// Sets the function to provide the comparer for the parameter, converting it to a comparer of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="TFrom">The type of the original comparer.</typeparam>
    /// <param name="comparerFromFunc">The function to provide the original comparer.</param>
    /// <param name="comparerToFunc">The function to convert the original comparer to a comparer of type <typeparamref name="T"/>.</param>
    /// <param name="comparerParameterName">The parameter's comparer name. Do not set this value as it's set at compile-time through <see cref="CallerArgumentExpressionAttribute"/>.</param>
    /// <returns>The current instance of the builder.</returns>
    public RegisterParameterBuilder<T> WithComparer<TFrom>(Func<IEqualityComparer<TFrom>> comparerFromFunc, Func<IEqualityComparer<TFrom>, IEqualityComparer<T>> comparerToFunc, [CallerArgumentExpression(nameof(comparerFromFunc))] string? comparerParameterName = null)
    {
        _comparer = new ParameterEqualityComparerTransformSwappable<TFrom, T>(comparerFromFunc, comparerToFunc);
        _comparerParameterName = comparerParameterName;

        return this;
    }

    /// <summary>
    /// Builds the parameter state.
    /// </summary>
    /// <returns>The created parameter state.</returns>
    internal ParameterStateInternal<T> Attach() => _parameterStateLazy.Value;

    private ParameterStateInternal<T> CreateParameterState()
    {
        ArgumentNullException.ThrowIfNull(_parameterName);

        var parameterState = ParameterStateInternal<T>.Attach(
            new ParameterMetadata(_parameterName, _handlerName, _comparerParameterName),
            _getParameterValueFunc ?? throw new ArgumentNullException(nameof(_getParameterValueFunc)),
            _eventCallbackFunc,
            _parameterChangedHandler,
            _comparer);

        return parameterState;
    }

    /// <inheritdoc />
    bool IParameterBuilderAttach.IsAttached => _parameterStateLazy.IsValueCreated;

    /// <inheritdoc />
    IParameterComponentLifeCycle IParameterBuilderAttach.Attach() => Attach();

    /// <summary>
    /// Implicitly converts a <see cref="RegisterParameterBuilder{T}"/> object to a <see cref="ParameterState{T}"/> object by building it.
    /// </summary>
    /// <param name="builder">The <see cref="RegisterParameterBuilder{T}"/> object to convert.</param>
    /// <returns>The created <see cref="ParameterState{T}"/> object.</returns>
    public static implicit operator ParameterState<T>(RegisterParameterBuilder<T> builder) => builder.Attach();
}
