// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// An observer that wraps a synchronous Action callback for event notifications.
/// </summary>
internal sealed class EventListenerLambdaObserver : IEventListenerObserver
{
    private readonly Action<object> _lambda;

    /// <inheritdoc />
    public Guid SubscriptionId { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventListenerLambdaObserver"/> class.
    /// </summary>
    /// <param name="subscriptionId">The unique subscription identifier.</param>
    /// <param name="lambda">The synchronous callback to invoke when the event occurs.</param>
    public EventListenerLambdaObserver(Guid subscriptionId, Action<object> lambda)
    {
        SubscriptionId = subscriptionId;
        _lambda = lambda;
    }

    /// <inheritdoc />
    public Task NotifyEventOccurredAsync(object eventArgs)
    {
        _lambda(eventArgs);
        return Task.CompletedTask;
    }
}
