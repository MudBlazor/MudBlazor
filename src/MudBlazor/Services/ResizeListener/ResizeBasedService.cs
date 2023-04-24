// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
#nullable enable
    public abstract class ResizeBasedService<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] TSelf, TInfo, TAction, TTaskOption> : IAsyncDisposable
        where TSelf : class
        where TInfo : SubscriptionInfo<TAction, TTaskOption>
    {
        protected SemaphoreSlim Semaphore = new(1, 1);

        protected Dictionary<Guid, TInfo> Listeners { get; } = new();

        protected IJSRuntime JsRuntime { get; init; }

        protected DotNetObjectReference<TSelf>? DotNetRef { get; set; }

        public ResizeBasedService(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }

        [Obsolete($"Use {nameof(UnsubscribeAsync)} instead. This will be removed in v7")]
        public Task<bool> Unsubscribe(Guid subscriptionId) => UnsubscribeAsync(subscriptionId);

        public async Task<bool> UnsubscribeAsync(Guid subscriptionId)
        {
            if (DotNetRef is null)
            {
                return false;
            }

            var info = Listeners.FirstOrDefault(x => x.Value.ContainsSubscription(subscriptionId));
            if ((info.Key, info.Value) == default)
            {
                return false;
            }

            try
            {
                await Semaphore.WaitAsync();

                var isLastSubscriber = info.Value.RemoveSubscription(subscriptionId);
                if (isLastSubscriber)
                {
                    if (Listeners.ContainsKey(info.Key))
                    {
                        Listeners.Remove(info.Key);

                        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudResizeListenerFactory.cancelListener", info.Key);
                    }
                }

                if (Listeners.Count == 0)
                {
                    DotNetRef.Dispose();
                    DotNetRef = null;
                }
            }
            finally
            {
                Semaphore.Release();
            }

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            if (DotNetRef is null) { return; }
            if (Listeners.Count == 0) { return; }

            var ids = Listeners.Keys.ToArray();
            Listeners.Clear();

            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudResizeListenerFactory.cancelListeners", ids);

            DotNetRef.Dispose();
        }
    }
}
