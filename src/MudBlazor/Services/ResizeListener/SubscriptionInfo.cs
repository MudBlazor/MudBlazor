// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace MudBlazor.Services
{
#nullable enable
    [Obsolete("This will be removed in v7.")]
    public class SubscriptionInfo<TAction,TOption>
    {
        private readonly Dictionary<Guid, Action<TAction>> _subscriptions;
        public TOption Option { get; init; }

        public SubscriptionInfo(TOption options)
        {
            Option = options;
            _subscriptions = new();
        }

        public Guid AddSubscription(Action<TAction> action)
        {
            var id = Guid.NewGuid();
            _subscriptions.TryAdd(id, action);

            return id;
        }

        public bool ContainsSubscription(Guid listenerId) => _subscriptions.ContainsKey(listenerId);

        public bool RemoveSubscription(Guid listenerId)
        {
            if (_subscriptions.ContainsKey(listenerId))
            {
                _subscriptions.Remove(listenerId);
            }
            return _subscriptions.Count == 0;
        }

        public void InvokeCallbacks(TAction browserWindowSize)
        {
            foreach (var item in _subscriptions.Values)
            {
                item.Invoke(browserWindowSize);
            }
        }
    }

    [Obsolete("This will be removed in v7.")]
    public class ResizeServiceSubscriptionInfo : SubscriptionInfo<BrowserWindowSize,ResizeOptions>
    {
        public ResizeServiceSubscriptionInfo(ResizeOptions options) : base(options)
        {
        }
    }

    [Obsolete("This will be removed in v7.")]
    public class BreakpointServiceSubscriptionInfo : SubscriptionInfo<Breakpoint, ResizeOptions>
    {
        public BreakpointServiceSubscriptionInfo(ResizeOptions options) : base(options)
        {
        }
    }
}
