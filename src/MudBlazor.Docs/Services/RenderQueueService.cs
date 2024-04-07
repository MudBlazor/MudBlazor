// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Components;

namespace MudBlazor.Docs.Services
{
    public interface IRenderQueueService
    {
        int Capacity { get; }

        ValueTask Enqueue(QueuedContent component);

        Task WaitUntilEmpty();

        void Clear();
    }

    public class RenderQueueService : IRenderQueueService
    {
        private TaskCompletionSource _emptyQueueTcs;
        private readonly ConcurrentQueue<QueuedContent> _queue = new();

        public int Capacity { get; init; } = 3;

        public void Clear()
        {
            _queue.Clear();
            _emptyQueueTcs?.TrySetResult();
            _emptyQueueTcs = null;
        }

        public async ValueTask Enqueue(QueuedContent component)
        {
            _queue.Enqueue(component);
            component.Rendered = EventCallback.Factory.Create(this, async () => await RenderNext());
            component.Disposed = EventCallback.Factory.Create(this, async () => await RenderNext());
            if (_queue.Count <= Capacity)
                await component.RenderAsync();
        }

        private async ValueTask RenderNext()
        {
            QueuedContent component;
            while (_queue.TryDequeue(out component) && (component.IsDisposed || component.IsRendered))
            {
                // Continue looking for an unrendered component
            }
            if (component != null)
            {
                await Task.Delay(20); // Wait a moment for prior renders to complete
                await component.RenderAsync();
            }
            if (_emptyQueueTcs != null && _queue.IsEmpty)
            {
                _emptyQueueTcs.TrySetResult();
                _emptyQueueTcs = null;
            }
        }

        public Task WaitUntilEmpty()
        {
            if (_queue.IsEmpty)
                return Task.CompletedTask;
            _emptyQueueTcs ??= new TaskCompletionSource();
            return _emptyQueueTcs.Task;
        }
    }
}
