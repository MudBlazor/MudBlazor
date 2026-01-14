// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MudBlazor.Docs.Components;

namespace MudBlazor.Docs.Services
{
    public interface IRenderQueueService
    {
        int Capacity { get; }

        void Enqueue(QueuedContent component);

        Task WaitUntilEmpty();

        void Clear();
    }

    public class RenderQueueService : IRenderQueueService
    {
        private TaskCompletionSource _tcs;
        private readonly Queue<QueuedContent> _queue = new();
        private const int BatchSize = 3; // Render 3 components at a time

        public int Capacity { get; init; }

        public RenderQueueService()
        {
            Capacity = 10;
        }

        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
                _tcs?.TrySetResult();
                _tcs = null;
            }
        }

        void IRenderQueueService.Enqueue(QueuedContent component)
        {
            bool renderImmediately;
            lock (_queue)
            {
                renderImmediately = _queue.Count == 0;
                _queue.Enqueue(component);
                component.Rendered += OnComponentRendered;
                component.Disposed += OnComponentDisposed;
            }
            if (renderImmediately)
                component.Render();
        }

        private void RenderNext()
        {
            List<QueuedContent> componentsToRender = new();
            lock (_queue)
            {
                // Get a batch of components to render
                int batchCount = 0;
                while (_queue.Count > 0 && batchCount < BatchSize)
                {
                    var component = _queue.Dequeue();
                    if (component.IsDisposed || component.IsRendered)
                    {
                        component.Rendered -= OnComponentRendered;
                        component.Disposed -= OnComponentDisposed;
                        continue;
                    }
                    componentsToRender.Add(component);
                    batchCount++;
                }
                if (componentsToRender.Count == 0)
                {
                    _tcs?.TrySetResult();
                    _tcs = null;
                    return;
                }
            }
            // Render all components in the batch
            // Note: QueuedContent.Render() uses InvokeAsync for thread-safe state changes,
            // so calling multiple renders in sequence is safe
            foreach (var component in componentsToRender)
            {
                component.Render();
            }
        }

        private void OnComponentRendered(QueuedContent component)
        {
            RenderNext();
        }

        private void OnComponentDisposed(QueuedContent component)
        {
            RenderNext();
        }

        public Task WaitUntilEmpty()
        {
            lock (_queue)
            {
                if (_queue.Count == 0)
                    return Task.CompletedTask;
                if (_tcs == null)
                    _tcs = new TaskCompletionSource();
                return _tcs.Task;
            }
        }
    }
}
