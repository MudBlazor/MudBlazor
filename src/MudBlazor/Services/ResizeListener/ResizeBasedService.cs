// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor.Services
{
    public abstract class ResizeBasedService<TSelf, TInfo, TAction, TaskOption> : IAsyncDisposable
        where TSelf : class
        where TInfo : SubscriptionInfo<TAction, TaskOption>
    {
        protected SemaphoreSlim Semaphore = new(1, 1);

        protected Dictionary<Guid, TInfo> Listeners { get; } = new();
        protected IJSRuntime JsRuntime { get; init; }
        protected DotNetObjectReference<TSelf> DotNetRef { get; set; }

        public ResizeBasedService(IJSRuntime jsRuntime)
        {
            JsRuntime = jsRuntime;
        }


        public async Task<bool> Unsubscribe(Guid subscriptionId)
        {
            if (DotNetRef == null)
            {
                return false;
            }

            var info = Listeners.FirstOrDefault(x => x.Value.ContainsSubscription(subscriptionId) == true);
            if (info.Value == null)
            {
                return false;
            }

            try
            {
                await Semaphore.WaitAsync();

                var isLastSubscriber = info.Value.RemoveSubscription(subscriptionId);
                if (isLastSubscriber == true)
                {
                    Listeners.Remove(info.Key);

                    await JsRuntime.InvokeVoidAsyncWithErrorHandling($"mudResizeListenerFactory.cancelListener", info.Key);
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
            if (DotNetRef == null) { return; }
            if (Listeners.Count == 0) { return; }

            var ids = Listeners.Keys.ToArray();
            Listeners.Clear();

            await JsRuntime.InvokeVoidAsyncWithErrorHandling($"mudResizeListenerFactory.cancelListeners", ids);

            DotNetRef.Dispose();
        }
    }
}
