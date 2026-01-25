// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// An observer that wraps an asynchronous Func callback for event notifications.
/// </summary>
internal sealed class EventListenerLambdaTaskObserver : IEventListenerObserver
{
    private readonly Func<object, Task> _lambda;

    /// <inheritdoc />
    public Guid SubscriptionId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventListenerLambdaTaskObserver"/> class.
    /// </summary>
    /// <param name="subscriptionId">The unique subscription identifier.</param>
    /// <param name="lambda">The asynchronous callback to invoke when the event occurs.</param>
    public EventListenerLambdaTaskObserver(Guid subscriptionId, Func<object, Task> lambda)
    {
        SubscriptionId = subscriptionId;
        _lambda = lambda;
    }

    /// <inheritdoc />
    public Task NotifyEventOccurredAsync(object eventArgs)
    {
        return _lambda(eventArgs);
    }
}
