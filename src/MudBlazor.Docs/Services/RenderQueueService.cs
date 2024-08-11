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

        public int Capacity { get; init; }

        public RenderQueueService()
        {
            Capacity = 3;
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

        private async void RenderNext()
        {
            QueuedContent componentToRender = null;
            lock (_queue)
            {
                while (_queue.Count > 0)
                {
                    var component = _queue.Dequeue();
                    if (component.IsDisposed || component.IsRendered)
                    {
                        component.Rendered -= OnComponentRendered;
                        component.Disposed -= OnComponentDisposed;
                        continue;
                    }
                    componentToRender = component;
                    break;
                }
                if (componentToRender == null)
                {
                    _tcs?.TrySetResult();
                    _tcs = null;
                    return;
                }
            }
            await Task.Delay(1);
            componentToRender.Render();
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
