// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
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
            var renderImmediately = _queue.IsEmpty;
            _queue.Enqueue(component);
            component.Rendered = EventCallback.Factory.Create(this, RenderNext);
            component.Disposed = EventCallback.Factory.Create(this, RenderNext);
            if (renderImmediately)
                await component.RenderAsync();
        }

        private async Task RenderNext()
        {
            while (_queue.TryDequeue(out var component))
            {
                if (component.IsDisposed || component.IsRendered)
                {
                    continue;
                }
                await component.RenderAsync();
            }
            if (_queue.IsEmpty)
            {
                _emptyQueueTcs?.TrySetResult();
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
