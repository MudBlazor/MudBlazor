﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Services
{
    public class SubscriptionInfo<TAction,TOption>
    {
        private Dictionary<Guid, Action<TAction>> _subscriptions;
        public TOption Option { get; init; }

        public SubscriptionInfo(TOption options)
        {
            _subscriptions = new();

            Option = options;
            _subscriptions = new();
        }

        public Guid AddSubscription(Action<TAction> action)
        {
            var id = Guid.NewGuid();
            _subscriptions.Add(id, action);

            return id;
        }

        public bool ContainsSubscription(Guid listenerId) => _subscriptions.ContainsKey(listenerId);

        public bool RemoveSubscription(Guid listenerId)
        {
            _subscriptions.Remove(listenerId);
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

    public class ResizeServiceSubscriptionInfo : SubscriptionInfo<BrowserWindowSize,ResizeOptions>
    {
        public ResizeServiceSubscriptionInfo(ResizeOptions options) : base(options)
        {
        }
    }

    public class BreakpointServiceSubscriptionInfo : SubscriptionInfo<Breakpoint, ResizeOptions>
    {
        public BreakpointServiceSubscriptionInfo(ResizeOptions options) : base(options)
        {
        }
    }
}
