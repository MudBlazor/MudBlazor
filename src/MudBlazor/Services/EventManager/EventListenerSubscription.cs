// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a subscription to a JavaScript event.
/// </summary>
internal sealed record EventListenerSubscription
{
    /// <summary>
    /// Gets the unique identifier for the subscription.
    /// </summary>
    public Guid SubscriptionId { get; }

    /// <summary>
    /// Gets the type of the event arguments.
    /// </summary>
    public Type EventType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventListenerSubscription"/> class.
    /// </summary>
    /// <param name="subscriptionId">The unique subscription identifier.</param>
    /// <param name="eventType">The type of the event arguments.</param>
    public EventListenerSubscription(Guid subscriptionId, Type eventType)
    {
        SubscriptionId = subscriptionId;
        EventType = eventType;
    }
}
